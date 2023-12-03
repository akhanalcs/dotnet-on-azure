using MunsonPickles.Web;
using MunsonPickles.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents() // 👈 Adds services required to server-side render components. Added by default.
    .AddInteractiveServerComponents(); // 👈 Stuff I added for Server Side Interactivity

// Add the config files. I added this.
var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? builder.Environment.EnvironmentName;

builder.Configuration
    .AddEnvironmentVariables()
    //.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);

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
app.UseAntiforgery();

app.MapRazorComponents<App>() // 👈 Discovers routable components and sets them up as endpoints. Added by default.
    .AddInteractiveServerRenderMode();  // 👈 Stuff I added for Server Side Interactivity

app.Run();