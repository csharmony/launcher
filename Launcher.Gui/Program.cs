using Avalonia;
using Avalonia.Media.Fonts;
using System;

namespace Launcher.Gui;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .ConfigureFonts(fontManager => fontManager.AddFontCollection(new EmbeddedFontCollection(
                new Uri("fonts:Geist", UriKind.Absolute),
                new Uri($"avares://{typeof(Program).Assembly.GetName().Name}/Assets/Fonts", UriKind.Absolute))))
            .LogToTrace();
}
