import * as cdk from "aws-cdk-lib";
import { Construct } from "constructs";
import * as dynamodb from "aws-cdk-lib/aws-dynamodb";
import * as ec2 from "aws-cdk-lib/aws-ec2";
import * as ecr from 'aws-cdk-lib/aws-ecr';
import * as ecs from 'aws-cdk-lib/aws-ecs';
import * as ecsPatterns from 'aws-cdk-lib/aws-ecs-patterns';
import * as elbv2 from 'aws-cdk-lib/aws-elasticloadbalancingv2';
import * as iam from 'aws-cdk-lib/aws-iam';
import * as logs from 'aws-cdk-lib/aws-logs';
import * as route53 from 'aws-cdk-lib/aws-route53';
import { ContainerDefinition } from 'aws-cdk-lib/aws-ecs';

export interface BandLabCommandConstructProps {
  stackName: string;
  stage: string;
  dockerImageTagForWebApi: string;
}

export class BandLabApiConstruct extends Construct {
  public postsTable: dynamodb.Table;
  constructor(
    scope: Construct,
    id: string,
    props: BandLabCommandConstructProps
  ) {
    super(scope, id);

    this.postsTable = new dynamodb.Table(
      this,
      `${props.stackName}-posts-data`,
      {
        tableName: `${props.stackName}-posts-data`,
        partitionKey: { name: "PK", type: dynamodb.AttributeType.STRING },
        sortKey: { name: "SK", type: dynamodb.AttributeType.STRING },
        billingMode: dynamodb.BillingMode.PAY_PER_REQUEST,
        encryption: dynamodb.TableEncryption.DEFAULT,
        pointInTimeRecovery: true,
        removalPolicy:
          props.stage == "master"
            ? cdk.RemovalPolicy.RETAIN
            : cdk.RemovalPolicy.DESTROY,
      }
    );

    const needoVpc = new ec2.Vpc(this, "bandLabVpc");

    // ECS Cluster
    const cluster = new ecs.Cluster(
      this,
      props.stackName + "band-lab-api-web-cluster",
      {
        vpc: needoVpc,
        clusterName: "band-lab-api-web-cluster",
      }
    );

    // ECS Task Definition
    const webTaskDefinition = new ecs.FargateTaskDefinition(
      this,
      props.stackName + "web-task",
      {
        memoryLimitMiB: 1024,
        cpu: 256,
      }
    );

    const webProxyRepo = ecr.Repository.fromRepositoryName(
      this,
      props.stackName + "web-proxy-repo",
      "bandlab"
    );

    const ecrPolicyStatement = new iam.PolicyStatement({
      effect: iam.Effect.ALLOW,
      actions: ["*"],
      resources: [webProxyRepo.repositoryArn],
    });

    webTaskDefinition.taskRole?.attachInlinePolicy(
      new iam.Policy(this, props.stackName + "web-task-policy", {
        policyName: "web-task-policy",
        statements: [
          ecrPolicyStatement,
        ],
      })
    );

    const logGroupWeb = new logs.LogGroup(
      this,
      props.stackName + "web-host-logs",
      {
        logGroupName: "/aws/ecs/web-host",
        retention: logs.RetentionDays.THREE_MONTHS,
        removalPolicy: cdk.RemovalPolicy.DESTROY,
      }
    ) as logs.ILogGroup;

    const webContainer = new ContainerDefinition(
      this,
      props.stackName + "web-container",
      {
        image: ecs.ContainerImage.fromEcrRepository(
          webProxyRepo,
          props.dockerImageTagForWebApi,
        ),
        memoryLimitMiB: 512,
        taskDefinition: webTaskDefinition,
        environment: {
        },
        essential: true,
        logging: ecs.LogDriver.awsLogs({
          streamPrefix: "web-host",
          logGroup: logGroupWeb,
        }),
      }
    );

    const portMappings: ecs.PortMapping[] = [
      { containerPort: 80, hostPort: 80 },
    ];
    webContainer.addPortMappings(...portMappings);

    const hostedZone = route53.HostedZone.fromLookup(this, props.stackName + 'bandlab-hosted-zone', {
      domainName: "api.needo.com.au",
      privateZone: false,
    });

    const loadBalancer = new elbv2.ApplicationLoadBalancer(
      this,
      props.stackName + "web-alb",
      {
        vpc: needoVpc,
        internetFacing: true,
      }
    );

    const httpListener = loadBalancer.addListener(
      props.stackName + "web-http-listener",
      {
        protocol: elbv2.ApplicationProtocol.HTTP,
        port: 80,
        defaultAction: elbv2.ListenerAction.fixedResponse(403, {
          messageBody: "Forbidden",
        }),
      }
    );

    httpListener.addAction("HttpRedirect", {
      action: elbv2.ListenerAction.redirect({
        protocol: "HTTPS",
        host: "#{host}",
        path: "/#{path}",
        query: "#{query}",
        port: "443",
      }),
    });

      // ECS Service with Application Load Balancer (SSL Termination)
    const loadBalancedFargateService =
      new ecsPatterns.ApplicationLoadBalancedFargateService(
        this,
        props.stackName + "web-service",
        {
          serviceName: "web-service",
          cluster,
          protocol: elbv2.ApplicationProtocol.HTTPS,
          memoryLimitMiB: 1024,
          cpu: 512,
          taskDefinition: webTaskDefinition,
          publicLoadBalancer: true,
          assignPublicIp: false,
          loadBalancer: loadBalancer,
          desiredCount: 1,
          listenerPort: 443,          
          redirectHTTP: false,
          domainZone: hostedZone,
          domainName: "api.needo.com.au",
        }
      );

    const autoScalingGroup =
      loadBalancedFargateService.service.autoScaleTaskCount({
        minCapacity: 1,
        maxCapacity: 2,
      });

    autoScalingGroup.scaleOnCpuUtilization("bandlab-web-cpu-scaling", {
      targetUtilizationPercent: 90,
      scaleInCooldown: cdk.Duration.seconds(60),
      scaleOutCooldown: cdk.Duration.seconds(60),
    });
  }
}
