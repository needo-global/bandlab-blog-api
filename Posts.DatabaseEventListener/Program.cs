using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Microsoft.Extensions.DependencyInjection;
using Posts.DatabaseEventListener;
using Posts.DatabaseEventListener.Handler.Abstract;
using Posts.DatabaseEventListener.Extensions;

var startup = new Startup();

var handler = async (DynamoDBEvent ddbEvent, ILambdaContext lambdaContext) =>
{
    using var scope = startup.ServiceProvider.CreateScope();

    var streamsEventResponse = new StreamsEventResponse();

    if (ddbEvent.Records == null)
    {
        // TODO Logging
        return streamsEventResponse;
    }

    var processingActions = scope.ServiceProvider.GetRequiredService<IEnumerable<IHandlerStrategy>>();
    var batchItemFailures = new List<StreamsEventResponse.BatchItemFailure>();

    foreach (var record in ddbEvent.Records)
    {
        try
        {
            await HandlerStreamRecord(record, processingActions);
        }
        catch (Exception ex)
        {
            // TODO Logging
            batchItemFailures.Add(new StreamsEventResponse.BatchItemFailure { ItemIdentifier = record.Dynamodb.SequenceNumber });
        }
    }

    if (batchItemFailures.Any())
    {
        streamsEventResponse.BatchItemFailures = batchItemFailures;
    }

    return streamsEventResponse;
};

async Task HandlerStreamRecord(DynamoDBEvent.DynamodbStreamRecord record, IEnumerable<IHandlerStrategy> processingActions)
{
    if (record.EventName.Equals("INSERT"))
    {
        var type = record.Dynamodb.NewImage["Type"].S;
        var action = processingActions.FirstOrDefault(a => a.Type.Equals(type));

        if (action == null) throw new Exception($"The action cannot be found for type {type}");
        
        switch (type)
        {
            case "POST":
                await action.Process(record.Dynamodb.NewImage.ToPost());
                break;
            case "COMMENT":
                await action.Process(record.Dynamodb.NewImage.ToComment());
                break;
        }
    }
    else if (record.EventName.Equals("REMOVE"))
    {
        var type = record.Dynamodb.OldImage["Type"].S;
        var action = processingActions.FirstOrDefault(a => a.Type.Equals(type));

        if (action == null) throw new Exception($"The action cannot be found for type {type}");

        if (type.Equals("COMMENT"))
        {
            await action.Process(record.Dynamodb.OldImage.ToComment());
        }
    }
}

await LambdaBootstrapBuilder.Create(handler, new DefaultLambdaJsonSerializer())
        .Build()
        .RunAsync();
