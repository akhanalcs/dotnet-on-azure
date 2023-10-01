using MunsonPickles.Web.New.Components;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddServerComponents();

// Azure AD B2C setup - Start
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAdB2C"));

// Can I get away without adding the Controller bits? I just want to use Minimal APIs in .NET 8
builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI();

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy
    options.FallbackPolicy = options.DefaultPolicy;
});

// Can I get away without adding the Razor Pages bits here? I just want to use Blazor Components in .NET 8
builder.Services.AddRazorPages();
builder.Services.AddMicrosoftIdentityConsentHandler();
//Configuring appsettings section AzureAdB2C, into IOptions
builder.Services.Configure<OpenIdConnectOptions>(builder.Configuration.GetSection("AzureAdB2C"));
//builder.Services.AddAntiforgery();
// Azure AD B2C setup - End

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

// Azure AD B2C setup - Start
app.UseAuthentication();
app.UseAuthorization();
//app.UseAntiforgery();
// Can I get away without adding the Controller bits? I just want to use Minimal APIs in .NET 8
app.MapControllers();
// Azure AD B2C setup - End

app.MapRazorComponents<App>()
    .AddServerRenderMode();

app.Run();
