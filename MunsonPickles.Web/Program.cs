using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using MunsonPickles.Web;
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

// AZURE ADB2C Setup - Start
//var initialScopes = builder.Configuration["DownstreamApi:Scopes"]?.Split(' ');
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAdB2C"));
    //.EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
    //.AddInMemoryTokenCaches();

builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI();

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy
    options.FallbackPolicy = options.DefaultPolicy;
});

/*builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
    // Handling SameSite cookie according to https://learn.microsoft.com/aspnet/core/security/samesite?view=aspnetcore-3.1
    options.HandleSameSiteCookieCompatibility();
});*/

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor()
    .AddMicrosoftIdentityConsentHandler();
//builder.Services.AddMicrosoftIdentityConsentHandler();

//Configuring appsettings section AzureAdB2C, into IOptions
builder.Services.Configure<OpenIdConnectOptions>(builder.Configuration.GetSection("AzureAdB2C"));

// AZURE ADB2C Setup - End

// Add my services. I added this.
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ReviewService>();

builder.Services.AddHttpClient();

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
app.UseRouting();

// Add the Microsoft Identity Web cookie policy
//app.UseCookiePolicy();//Some comment

// Add the ASP.NET Core authentication service
//app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapRazorComponents<App>() // 👈 Discovers routable components and sets them up as endpoints. Added by default.
    .AddServerRenderMode();  // 👈 Stuff I added for Server Side Interactivity

app.Run();