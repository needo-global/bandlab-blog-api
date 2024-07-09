import * as cdk from 'aws-cdk-lib';
import {Construct} from 'constructs';
import * as logs from 'aws-cdk-lib/aws-logs';
import context from "../../helpers/context";

export class LambdaLogGroup extends Construct {
    constructor(scope: Construct, id: string, functionName: string) {
        super(scope, id);

        const stage = context.getStage(scope);
        const retention = stage == "master" ? logs.RetentionDays.INFINITE : logs.RetentionDays.ONE_MONTH;
        
        // creates the loggroup
        const logGroup = new logs.LogGroup(scope, `${functionName}-log-group`, {
            logGroupName: `/aws/lambda/${functionName}`,
            retention: retention,
            removalPolicy: cdk.RemovalPolicy.DESTROY
        });
    }
}