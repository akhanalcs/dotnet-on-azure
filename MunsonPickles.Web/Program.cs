using Microsoft.EntityFrameworkCore;
using MunsonPickles.Web;
using MunsonPickles.Web.Data;
using MunsonPickles.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents();

// Add my services
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ReviewService>();

var azSqlDbConnection = "";
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddEnvironmentVariables().AddJsonFile("appsettings.Development.json");
    azSqlDbConnection = builder.Configuration.GetConnectionString("Default");
}
else
{
    azSqlDbConnection = Environment.GetEnvironmentVariable("Default");
}

builder.Services.AddDbContext<PickleDbContext>(options =>
{
    options.UseSqlServer(azSqlDbConnection);
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

app.MapRazorComponents<App>();

// Add my custom pipeline stuffs

app.Run();