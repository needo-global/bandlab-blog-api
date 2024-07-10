import * as cdk from "aws-cdk-lib";
import { Construct } from "constructs";
import * as dynamodb from "aws-cdk-lib/aws-dynamodb";
import * as ec2 from "aws-cdk-lib/aws-ec2";
import * as ecr from "aws-cdk-lib/aws-ecr";
import * as ecs from "aws-cdk-lib/aws-ecs";
import * as elbv2 from "aws-cdk-lib/aws-elasticloadbalancingv2";
import * as route53 from "aws-cdk-lib/aws-route53";
import * as acm from "aws-cdk-lib/aws-certificatemanager";

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
        stream: dynamodb.StreamViewType.NEW_AND_OLD_IMAGES,
      }
    );

    const vpc = new ec2.Vpc(this, "bandLabVpc");

    const ecrRepo = ecr.Repository.fromRepositoryName(
      this,
      props.stackName + "web-proxy-repo",
      "bandlab"
    );

    const cluster = new ecs.Cluster(
      this,
      props.stackName + "band-lab-api-web-cluster",
      {
        vpc: vpc,
        clusterName: "band-lab-api-web-cluster",
      }
    );

    const taskDefinition = new ecs.FargateTaskDefinition(
      this,
      props.stackName + "web-task",
      {
        cpu: 256,
        memoryLimitMiB: 1024,
      }
    );

    const container = taskDefinition.addContainer(
      props.stackName + "web-container",
      {
        image: ecs.ContainerImage.fromEcrRepository(
          ecrRepo,
          props.dockerImageTagForWebApi
        ),
        environment: {},
        logging: new ecs.AwsLogDriver({
          streamPrefix: props.stackName + "web-service",
        }),
      }
    );

    container.addPortMappings({
      containerPort: 80,
    });

    const fargateService = new ecs.FargateService(
      this,
      props.stackName + "web-service",
      {
        taskDefinition,
        cluster,
        desiredCount: 1,
        serviceName: props.stackName + "web-service",
        propagateTags: ecs.PropagatedTagSource.SERVICE,
      }
    );

    const autoScalingGroup = fargateService.autoScaleTaskCount({
      minCapacity: 1,
      maxCapacity: 2,
    });

    autoScalingGroup.scaleOnCpuUtilization("bandlab-web-cpu-scaling", {
      targetUtilizationPercent: 90,
      scaleInCooldown: cdk.Duration.seconds(60),
      scaleOutCooldown: cdk.Duration.seconds(60),
    });

    const certificate = new acm.Certificate(
      scope,
      props.stackName + "bandlab-certificate",
      {
        domainName: "api.needo.com.au",
        validation: acm.CertificateValidation.fromDns(),
      }
    );

    const loadBalancer = new elbv2.ApplicationLoadBalancer(
      scope,
      props.stackName + "web-alb",
      {
        vpc,
        internetFacing: true,
      }
    );

    const listener1 = new elbv2.ApplicationListener(
      scope,
      props.stackName + "bandlab-http-listener",
      {
        loadBalancer,
        port: 80,
        protocol: elbv2.ApplicationProtocol.HTTP,
      }
    );

    listener1.addAction("HttpsRedirect", {
      action: elbv2.ListenerAction.redirect({
        permanent: true,
        protocol: "HTTPS",
        port: "443",
        host: "#{host}",
        path: "/#{path}",
        query: "#{query}",
      }),
    });

    const listener2 = new elbv2.ApplicationListener(
      scope,
      props.stackName + "web-http-listener",
      {
        loadBalancer,
        port: 443,
        protocol: elbv2.ApplicationProtocol.HTTPS,
        certificates: [certificate],
      }
    );

    listener2.addTargets("HttpsListenerTarget", {
      targets: [fargateService],
      healthCheck: {
        path: "/",
        healthyHttpCodes: "200,302",
      },
      protocol: elbv2.ApplicationProtocol.HTTP,
      port: 80,
    });

    const fullDomainName = "api.needo.com.au";

    const domainZone = route53.HostedZone.fromLookup(
      scope,
      props.stackName + "bandlab-hosted-zone",
      {
        domainName: "needo.com.au",
      }
    );

    const recordSet = new route53.CfnRecordSet(
      scope,
      props.stackName + "bandlab-hosted-zone-records",
      {
        type: "A",
        name: `${fullDomainName}.`,
        aliasTarget: {
          dnsName: loadBalancer.loadBalancerDnsName,
          hostedZoneId: loadBalancer.loadBalancerCanonicalHostedZoneId,
        },
        hostedZoneId: domainZone.hostedZoneId,
      }
    );

    new cdk.CfnOutput(this, "URL", {
      value: `https://${fullDomainName}`,
    });
  }
}
