using Azure.Core;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Http.Json;
using System.Text.Json.Serialization;
using MunsonPickles.API.Data;
using MunsonPickles.API.Endpoints;
using MunsonPickles.API.Models;

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

// AZURE ADB2C Setup - Start
//https://learn.microsoft.com/en-us/aspnet/core/blazor/security/webassembly/hosted-with-azure-active-directory-b2c?view=aspnetcore-7.0&viewFallbackFrom=aspnetcore-8.0#authentication-service-support
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2C"));

//https://learn.microsoft.com/en-us/aspnet/core/blazor/security/webassembly/hosted-with-azure-active-directory-b2c?view=aspnetcore-7.0#configure-useridentityname
builder.Services.Configure<JwtBearerOptions>(
    JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters.NameClaimType = "name";
    });

var initialScopes = builder.Configuration["AzureAdB2C:Scopes"]?.Split(' ');

// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddMicrosoftIdentityWebApi(jwtBearerOptions =>
//     {
//         builder.Configuration.Bind("AzureAdB2C", jwtBearerOptions);
//         //https://learn.microsoft.com/en-us/aspnet/core/blazor/security/webassembly/hosted-with-azure-active-directory-b2c?view=aspnetcore-7.0#configure-useridentityname
//         jwtBearerOptions.TokenValidationParameters.NameClaimType = "name";
//     }, identityOptions =>
//     {
//         builder.Configuration.Bind("AzureAdB2C", identityOptions);
//     });

builder.Services.AddAuthorization();

// AZURE ADB2C Setup - End

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("user_read", policy =>
        policy
            //.RequireRole("user")
            .RequireClaim("scope", "read"));

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

//https://stackoverflow.com/a/77198035/8644294
//Matches request to an endpoint.
//Any middleware before this line won't know which endpoint will run eventually. (Any middleware after line after this line will know)
//The endpoint is always null before UseRouting is called.
//app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

//Add your endpoints after these 2 calls ☝️ to protect them

//This call should come AFTER calls to useAuth-N&Z
app.UseAntiforgery();

// Add my pipeline stuffs
app.MapProductEndpoints();
app.MapReviewEndpoints();
app.MapGet("/public", () => $"Time is {DateTime.UtcNow}.");

//The middleware after UseEndpoints execute only when no match is found
//app.UseEndpoints(_ => { }); //For eg: app.UseEndpoints(endpoint => endpoint.MapGet("/", () => "Hello World"));

//app.CreateDbIfNotExists();

app.Run();