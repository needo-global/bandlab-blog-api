using Posts.Domain.Abstract;
using Posts.Infrastructure.Repository;
using Posts.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddTransient<IPostService, PostService>();
builder.Services.AddTransient<IPostCommandRepository, PostCommandRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
