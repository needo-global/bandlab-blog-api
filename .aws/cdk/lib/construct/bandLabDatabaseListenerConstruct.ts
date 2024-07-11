import * as cdk from 'aws-cdk-lib';
import {Construct} from 'constructs';
import * as lambda from 'aws-cdk-lib/aws-lambda';
import * as iam from 'aws-cdk-lib/aws-iam';
import * as dynamodb from 'aws-cdk-lib/aws-dynamodb';
import { DynamoEventSource } from 'aws-cdk-lib/aws-lambda-event-sources';

export interface BandLabDatabaseListenerConstructProps {
    stackName: string;
    stage: string;
    postsTable: dynamodb.Table;
}

export class BandLabDatabaseListenerConstruct extends Construct {
    constructor(scope: Construct, id: string, props: BandLabDatabaseListenerConstructProps) {
        super(scope, id);
        
        const functionName = `${props.stackName}-database-listener`;
        
        // Defines the lambda function to reserve virtual accounts
        const databaseListenerFunction = new lambda.Function(this, functionName, {
            functionName: functionName,
            description: "This function acts as the handler for processing DynamoDB stream events",
            runtime: lambda.Runtime.DOTNET_6,
            code: lambda.Code.fromAsset("../../artifacts/posts-databaseeventlistener/posts-databaseeventlistener.zip"),
            handler: "Posts.DatabaseEventListener",
            memorySize: 1024,
            timeout: cdk.Duration.seconds(300),
            environment: {
                // TODO - Set up environment variables to avoid hardcoding in code
            },
        });

        databaseListenerFunction.addEventSource(new DynamoEventSource(props.postsTable,{
            startingPosition: lambda.StartingPosition.LATEST,
            retryAttempts: 1,
            batchSize: 10,
            bisectBatchOnError: true,
            reportBatchItemFailures: true,
          })) 

        // Define and add cloudwatch access for the reserve virtual accounts function
        const logSSMRolePolicyStatement = new iam.PolicyStatement({
            effect: iam.Effect.ALLOW,
            actions: ["logs:CreateLogGroup", "logs:CreateLogStream", "logs:PutLogEvents"],
            resources: ['*']
        });

        // Grant permissions for lambda to access other resources
        props.postsTable.grantReadWriteData(databaseListenerFunction);
        props.postsTable.grantStream(databaseListenerFunction);
        props.postsTable.grantStreamRead(databaseListenerFunction);
        databaseListenerFunction.addToRolePolicy(logSSMRolePolicyStatement);
    }
}
