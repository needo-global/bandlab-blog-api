import * as cdk from "aws-cdk-lib";
import { Construct } from "constructs";
import * as dynamodb from "aws-cdk-lib/aws-dynamodb";
import * as ec2 from "aws-cdk-lib/aws-ec2";
import * as ecr from "aws-cdk-lib/aws-ecr";
import * as ecs from "aws-cdk-lib/aws-ecs";
import * as elbv2 from "aws-cdk-lib/aws-elasticloadbalancingv2";
import * as route53 from "aws-cdk-lib/aws-route53";
import * as acm from "aws-cdk-lib/aws-certificatemanager";
import * as s3 from "aws-cdk-lib/aws-s3";
import * as iam from "aws-cdk-lib/aws-iam";

export interface BandLabCommandConstructProps {
  stackName: string;
  stage: string;
  dockerImageTagForWebApi: string;
}

export class BandLabApiConstruct extends Construct {
  public postsTable: dynamodb.Table;
  public postsImages: s3.Bucket;
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

    // this.postsTable.addGlobalSecondaryIndex({
    //   indexName: 'PostsByCommentCountIndex',
    //   partitionKey: {name: 'Type', type: dynamodb.AttributeType.STRING},
    //   sortKey: {name: 'CommentCount', type: dynamodb.AttributeType.NUMBER},
    //   projectionType: dynamodb.ProjectionType.INCLUDE,
    //   nonKeyAttributes: ['Id', 'Image', 'Caption', 'Creator', 'CreatedAt', 'CommentCount', 'RecentComments'],
    // });

    const dataBucketName = props.stage == 'master' ? "bandlab-post-data" : "bandlab-post-dev-data";
    this.postsImages = new s3.Bucket(this, `${props.stackName}-post-images`, {
      blockPublicAccess: new s3.BlockPublicAccess({ blockPublicPolicy: false, }),
      bucketName: dataBucketName,
      removalPolicy: props.stage == 'master' ? cdk.RemovalPolicy.RETAIN : cdk.RemovalPolicy.DESTROY,
      publicReadAccess: true,
      versioned: false,
      encryption: s3.BucketEncryption.S3_MANAGED,
      bucketKeyEnabled: false,
      enforceSSL: true,
      cors: [{
        allowedMethods: [s3.HttpMethods.GET],
        allowedOrigins: ['*'],
        allowedHeaders: ['Authorization', 'Content-Length'],
        exposedHeaders: [],
        id: 'cors-data',
        maxAge: 3000,
      }],
      transferAcceleration: true,
    });

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
      },
    );

    taskDefinition.taskRole?.addToPrincipalPolicy(new iam.PolicyStatement({
      effect: iam.Effect.ALLOW,
      sid: "s3BucketActions",
      actions: ["s3:ListAllMyBuckets", "s3:ListBucket"],
      resources: [`*`],
    }));

    taskDefinition.taskRole?.addToPrincipalPolicy(new iam.PolicyStatement({
      effect: iam.Effect.ALLOW,
      sid: "s3ObjectActions",
      actions: ["s3:*Object"],
      resources: [`*`],
    }));

    taskDefinition.taskRole?.addToPrincipalPolicy(new iam.PolicyStatement({
      effect: iam.Effect.ALLOW,
      sid: "DynamoDBActions",
      actions: ["dynamodb:*"],
      resources: [this.postsTable.tableArn, `${this.postsTable.tableArn}/index/*`],
    }));

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
        domainName: "api.needo.com.au",
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
