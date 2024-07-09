import * as cdk from 'aws-cdk-lib';
import { Construct } from 'constructs';
import { BandLabApiConstruct } from './construct/bandLabApiConstruct';

export interface BandLabApiStackProps extends cdk.StackProps {
  serviceName: string;
  stackName: string;
  stage: string;
  dockerImageTagForWebApi: string;
}

export class BandLabApiStack extends cdk.Stack {
  constructor(scope: Construct, id: string, props: BandLabApiStackProps) {
    super(scope, id, props);
    
    const bandLabCommandResources = new BandLabApiConstruct(this, "bandlab-command", {
      stackName: props.stackName,
      stage: props.stage,
      dockerImageTagForWebApi: props.dockerImageTagForWebApi,
    });
    
  }
}