#nullable enable

#if ANDROID || IOS || MACCATALYST || WINDOWS
using BauToolKit.Application;
using BauToolKit.Infrastructure;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;

namespace BauToolKit.Mobile.Host;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>();

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        // Register BauToolKit layers
        builder.Services.AddBauToolKitApplication();
        builder.Services.AddBauToolKitInfrastructure();

        MauiApp app = builder.Build();

        // Seed demo data for mobile offline/local use
        using (IServiceScope scope = app.Services.CreateScope())
        {
            var seeder = scope.ServiceProvider.GetRequiredService<SampleDataSeeder>();
            seeder.SeedAsync().GetAwaiter().GetResult();
        }

        return app;
    }
}
#else
namespace BauToolKit.Mobile.Host;

public static class Program
{
    public static void Main(string[] args)
    {
        // Entrypoint when building on platforms without MAUI workloads installed
    }
}
#endif
