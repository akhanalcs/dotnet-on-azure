using Azure.Core;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using MunsonPickles.Web;
using MunsonPickles.Web.Data;
using MunsonPickles.Web.Models;
using MunsonPickles.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents() // 👈 Adds services required to server-side render components. Added by default.
    .AddServerComponents(); // 👈 Stuff I added for Server Side Interactivity

// Add the config files. I added this.
var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? builder.Environment.EnvironmentName;

builder.Configuration
    .AddEnvironmentVariables()
    //.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);

// Add my services. I added this.
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ReviewService>();

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
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.MapRazorComponents<App>() // 👈 Discovers routable components and sets them up as endpoints. Added by default.
    .AddServerRenderMode();  // 👈 Stuff I added for Server Side Interactivity

// Add my custom pipeline stuffs
//app.CreateDbIfNotExists();

app.Run();