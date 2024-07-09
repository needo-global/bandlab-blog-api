#!/usr/bin/env node
import "source-map-support/register";
import * as cdk from "aws-cdk-lib";
import context from "../lib/helpers/context";
import { BandLabApiStack } from "../lib/bandlab-posts-api-stack";

const app = new cdk.App();
const stackName = context.getStackName(app);
const stage = context.getStage(app);
const dockerImageTagForWebApi = context.getDockerImageTagForWebApi(app);

new BandLabApiStack(app, stackName, {
  serviceName: "bandlab-posts-api",
  stackName: stackName,
  stage,
  dockerImageTagForWebApi,
  env: {
    account: "231135948134",
    region: "ap-southeast-2",
  },
  /* If you don't specify 'env', this stack will be environment-agnostic.
   * Account/Region-dependent features and context lookups will not work,
   * but a single synthesized template can be deployed anywhere. */

  /* Uncomment the next line to specialize this stack for the AWS Account
   * and Region that are implied by the current CLI configuration. */
  // env: { account: process.env.CDK_DEFAULT_ACCOUNT, region: process.env.CDK_DEFAULT_REGION },

  /* Uncomment the next line if you know exactly what Account and Region you
   * want to deploy the stack to. */
  // env: { account: '123456789012', region: 'us-east-1' },

  /* For more information, see https://docs.aws.amazon.com/cdk/latest/guide/environments.html */
});
