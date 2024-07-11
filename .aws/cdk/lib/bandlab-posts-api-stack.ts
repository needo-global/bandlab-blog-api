import * as cdk from 'aws-cdk-lib';
import { Construct } from 'constructs';
import { BandLabApiConstruct } from './construct/bandLabApiConstruct';
import { BandLabDatabaseListenerConstruct } from './construct/bandLabDatabaseListenerConstruct';

export interface BandLabApiStackProps extends cdk.StackProps {
  serviceName: string;
  stackName: string;
  stage: string;
  dockerImageTagForWebApi: string;
}

export class BandLabApiStack extends cdk.Stack {
  constructor(scope: Construct, id: string, props: BandLabApiStackProps) {
    super(scope, id, props);
    
    const bandLabApiResources = new BandLabApiConstruct(this, "bandlab-api", {
      stackName: props.stackName,
      stage: props.stage,
      dockerImageTagForWebApi: props.dockerImageTagForWebApi,
    });

    const bandLabDatabaseListenerResources = new BandLabDatabaseListenerConstruct(this, "bandlab-databse-listener", {
      stackName: props.stackName,
      stage: props.stage,
      postsTable: bandLabApiResources.postsTable,
      postsImages: bandLabApiResources.postsImages,
    });
    
  }
}