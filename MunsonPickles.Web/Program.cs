using Microsoft.EntityFrameworkCore;
using MunsonPickles.Web;
using MunsonPickles.Web.Data;
using MunsonPickles.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents() // 👈 Adds services required to server-side render components. Added by default.
    .AddServerComponents(); // 👈 Stuff I added for Server Side Interactivity

// Add my services
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ReviewService>();

var azSqlDbConnection = "";
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddEnvironmentVariables().AddJsonFile("appsettings.Development.json");
    azSqlDbConnection = builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING");
}
else
{
    azSqlDbConnection = Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING");
}

builder.Services.AddDbContext<PickleDbContext>(options =>
{
    options.UseSqlServer(azSqlDbConnection, azConnOpts =>
    {
        azConnOpts.EnableRetryOnFailure();
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
app.CreateDbIfNotExists();

app.Run();