using Azure.Core;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using MunsonPickles.API.Data;
using MunsonPickles.API.Endpoints;
using MunsonPickles.API.Models;
using Microsoft.AspNetCore.Http.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add the config files. I added this.
var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? builder.Environment.EnvironmentName;

builder.Configuration
    .AddEnvironmentVariables()
    //.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddAntiforgery();

// Add my services. I added this.
var azSqlDbConnection = builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING");
// string? azSqlDbConnection;
// if (builder.Environment.IsDevelopment())
// {
//     azSqlDbConnection = builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING");
// }
// else
// {
//     azSqlDbConnection = Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING");
// }

builder.Services.AddDbContext<PickleDbContext>(options =>
{
    options.UseSqlServer(azSqlDbConnection, azConnOpts =>
    {
        azConnOpts.EnableRetryOnFailure();
    });
});

// Azure storage config
// If you want to use AzureStorageConfigOpts in your services. This simply populates the config into that object.
builder.Services.Configure<AzureStorageConfigOpts>(builder.Configuration.GetSection("AzureStorageConfig"));

var azStorageConnection = builder.Configuration["AzureStorageConfig:ConnectionString"]!;
builder.Services.AddAzureClients(azureBuilder =>
{
    azureBuilder.UseCredential(new DefaultAzureCredential());
    azureBuilder.AddBlobServiceClient(new Uri(azStorageConnection))
        .ConfigureOptions(opts =>
        {
            opts.Retry.MaxRetries = 3;
            opts.Retry.Mode = RetryMode.Exponential;
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

// Add my custom pipeline stuffs
app.MapProductEndpoints();
app.MapReviewEndpoints();
//app.CreateDbIfNotExists();

app.Run();