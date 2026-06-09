using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace Launcher.Helpers;

public static class Steam
{
    public static string? GetGamePath(int appId)
    {
        string? steamPath = GetSteamInstallPath();
        if (string.IsNullOrEmpty(steamPath))
            return null;

        string vdfPath = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf");
        if (!File.Exists(vdfPath))
            return null;

        List<string> libraryPaths = ParseLibraryFolders(vdfPath);

        foreach (string library in libraryPaths)
        {
            string manifestPath = Path.Combine(library, "steamapps", $"appmanifest_{appId}.acf");
            if (!File.Exists(manifestPath))
                continue;

            string? folderName = ParseInstallDir(manifestPath);
            if (string.IsNullOrEmpty(folderName))
                continue;

            string fullPath = Path.Combine(library, "steamapps", "common", folderName);
            if (Directory.Exists(fullPath))
                return fullPath;
        }

        return null;
    }

    private static string? GetSteamInstallPath()
    {
        if (OperatingSystem.IsWindows())
        {
            using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Valve\Steam") ??
                            Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Valve\Steam");
            return key?.GetValue("InstallPath")?.ToString();
        }

        return null;
    }

    private static List<string> ParseLibraryFolders(string vdfPath)
    {
        var paths = new List<string>();
        string fileContent = File.ReadAllText(vdfPath);

        var matches = Regex.Matches(fileContent, @"""path""\s+""([^""]+)""");
        foreach (Match match in matches)
        {
            string path = match.Groups[1].Value.Replace(@"\\", @"\");
            if (Directory.Exists(path))
                paths.Add(path);
        }

        return paths;
    }

    private static string? ParseInstallDir(string manifestPath)
    {
        string fileContent = File.ReadAllText(manifestPath);
        var match = Regex.Match(fileContent, @"""installdir""\s+""([^""]+)""");
        return match.Success ? match.Groups[1].Value : null;
    }
}
