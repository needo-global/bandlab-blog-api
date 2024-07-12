using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Microsoft.Extensions.DependencyInjection;
using Posts.DatabaseEventListener;
using Posts.DatabaseEventListener.Handler.Abstract;
using Posts.DatabaseEventListener.Extensions;
using Posts.Domain;

var startup = new Startup();

const string Insert = "INSERT";
const string Remove = "REMOVE";

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
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
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
    if (record.EventName.Equals(Insert))
    {
        var type = record.Dynamodb.NewImage["Type"].S;
        var action = processingActions.FirstOrDefault(a => a.Type.Equals(type));

        if (action == null) throw new Exception($"The action cannot be found for type {type}");
        
        switch (type)
        {
            case Constants.Post:
                await action.Process(record.Dynamodb.NewImage.ToPost(), Insert);
                break;
            case Constants.Comment:
                await action.Process(record.Dynamodb.NewImage.ToComment(), Insert);
                break;
        }
    }
    else if (record.EventName.Equals(Remove))
    {
        var type = record.Dynamodb.OldImage["Type"].S;
        var action = processingActions.FirstOrDefault(a => a.Type.Equals(type));

        if (action == null) throw new Exception($"The action cannot be found for type {type}");

        if (type.Equals(Constants.Comment))
        {
            await action.Process(record.Dynamodb.OldImage.ToComment(), Remove);
        }
    }
    else
    {
        /* This should be a post or comment modification.
           Ideally we should filter those events being forwarded to this lambda using a filter */
    }
}

await LambdaBootstrapBuilder.Create(handler, new DefaultLambdaJsonSerializer())
        .Build()
        .RunAsync();
