namespace MunsonPickles.Web.Data;

public static class Extensions
{
    public static void CreateDbIfNotExists(this IHost host)
    {
        // WebApplication in "WebApplication app = builder.Build();" is (Notice IHost below):
        // public sealed class WebApplication : IHost, IApplicationBuilder, IEndpointRouteBuilder, IAsyncDisposable
        // so it's like: "using var scope = app.Services.CreateAsync();" in Program.cs file
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var db = services.GetRequiredService<PickleDbContext>();
        db.Database.EnsureCreated();
        DbInitializer.InitializeDatabase(db);
    }
}