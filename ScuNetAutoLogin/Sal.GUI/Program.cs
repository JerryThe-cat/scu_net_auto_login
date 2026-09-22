using System;
using System.Linq;
using System.Runtime.Versioning;
using Avalonia;
using ConsoleAppFramework;
using Sal.GUI.CLICommand;
using ServiceLib.Manager;

namespace Sal.GUI;

internal sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        AppManager.Instance.InitApp();
        if (args.FirstOrDefault() == "cli")
        {
            // Run CLI command
            var app = ConsoleApp.Create();
            app.Add<MyCommands>();
            app.Run(args.Skip(1).ToArray());
            return;
        }

        // Run GUI
        if (OperatingSystem.IsLinux() && !IsEnvVarTrue("DISABLE_WAYLAND"))
        {
            BuildAvaloniaAppWithWayland().StartWithClassicDesktopLifetime(args);
            return;
        }

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .LogToTrace();

    [SupportedOSPlatform("linux")]
    public static AppBuilder BuildAvaloniaAppWithWayland()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .LogToTrace()
            .UseWaylandWithFallback();

    private static bool IsEnvVarTrue(string variableName)
    {
        var value = Environment.GetEnvironmentVariable(variableName);
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        value = value.Trim();
        return string.Equals(value, "1", StringComparison.OrdinalIgnoreCase)
               || string.Equals(value, "true", StringComparison.OrdinalIgnoreCase)
               || string.Equals(value, "yes", StringComparison.OrdinalIgnoreCase);
    }
}
