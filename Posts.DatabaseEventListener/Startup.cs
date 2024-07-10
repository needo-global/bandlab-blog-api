using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Posts.DatabaseEventListener.Handler;
using Posts.DatabaseEventListener.Handler.Abstract;
using Posts.Domain.Abstract;
using Posts.Infrastructure.Abstract;
using Posts.Infrastructure.Repository.Command;
using Posts.Infrastructure;
using Posts.Services;

namespace Posts.DatabaseEventListener;

public class Startup
{
    public Startup()
    {
        var configuration = ConfigureAppConfiguration();

        var services = new ServiceCollection();
        ConfigureServices(services, configuration);

        ServiceProvider = services.BuildServiceProvider();
    }

    public IServiceProvider ServiceProvider { get; }

    private static IConfiguration ConfigureAppConfiguration()
    {
        var configMgr = new ConfigurationManager()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true);

        configMgr.AddEnvironmentVariables();

        return configMgr.Build();
    }

    private static void ConfigureServices(ServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(configuration);
        services.AddTransient<IPostService, PostService>();
        services.AddTransient<IPostCommandRepository, PostCommandRepository>();
        services.AddTransient<IStorage, AwsS3FileStore>();
        services.AddDefaultAWSOptions(configuration.GetAWSOptions());
        services.AddAWSService<IAmazonDynamoDB>();
        services.AddSingleton<IDynamoDBContext, DynamoDBContext>();
        services.AddTransient<IHandlerStrategy, PostProcessingStrategy>();
        services.AddTransient<IHandlerStrategy, CommentProcessingStrategy>();
    }
}
