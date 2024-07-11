using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Microsoft.Extensions.DependencyInjection;
using Posts.DatabaseEventListener;
using Posts.DatabaseEventListener.Handler.Abstract;

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
    var type = record.Dynamodb.NewImage["Type"].S;

    if (record.EventName == "INSERT")
    {
        switch (type)
        {
            case "POST":
                Console.WriteLine("Inserted post");
                break;
            case "COMMENT":
                Console.WriteLine("Inserted comment");
                break;
        }
    }
    else if (record.EventName == "REMOVE" && type.Equals("COMMENT"))
    {
        Console.WriteLine("Removed comment");
    }
}

await LambdaBootstrapBuilder.Create(handler, new DefaultLambdaJsonSerializer())
        .Build()
        .RunAsync();
