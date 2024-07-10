using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.AspNetCore.Mvc;
using Posts.Api.Middleware.Error;
using Posts.Api.Middleware.Validation;
using Posts.Domain.Abstract;
using Posts.Infrastructure;
using Posts.Infrastructure.Abstract;
using Posts.Infrastructure.Repository.Command;
using Posts.Infrastructure.Repository.Query;
using Posts.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<ValidationFilterAttribute>();
builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
builder.Services.AddTransient<IPostService, PostService>();
builder.Services.AddTransient<IPostCommandRepository, PostCommandRepository>();
builder.Services.AddTransient<IPostQueryRepository, PostQueryRepository>();
builder.Services.AddTransient<IStorage, AwsS3FileStore>();
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddSingleton<IDynamoDBContext, DynamoDBContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();