using System.Diagnostics;
using System.Reflection;

namespace Launcher.Helpers;

public static class Update
{
    public static async Task CheckAsync()
    {
        if (Arguments.SkipUpdates)
        {
            Terminal.Warning("Skipping checking launcher updates. Things might not work properly!");
            return;
        }

        try
        {
            var release = await Api.GitHub.GetLatestRelease();
            if (!Version.TryParse(release.TagName, out var latestVersion))
                return;

            if (latestVersion.CompareTo(CurrentVersion()) <= 0)
                return;

            if (await Terminal.ConfirmAsync(
                    $"Update found (v{CurrentVersion()} -> v{latestVersion}). Would you like to update?"))
            {
                // disable browser output in linux terminal
                if (OperatingSystem.IsLinux())
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "xdg-open",
                        Arguments = "https://github.com/csharmony/launcher/releases/latest",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    });
                }
                else
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "https://github.com/csharmony/launcher/releases/latest",
                        UseShellExecute = true
                    });
                }

                Environment.Exit(0);
            }
            else
            {
                Terminal.Warning("You are not using the latest launcher version. Things might not work properly!");
            }
        }
        catch (Exception e)
        {
            Terminal.Error(
                "An error occurred while checking launcher updates. Are you connected to the Internet?");

            if (Debug.IsEnabled)
                Terminal.Debug(e);
        }
    }

    public static Version CurrentVersion()
    {
        // assembly versions always use Major.Minor.Build.Revision
        // we don't use and need Revision, so return new Version object without passing it
        var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version!;
        return new Version(assemblyVersion.Major, assemblyVersion.Minor, assemblyVersion.Build);
    }
}