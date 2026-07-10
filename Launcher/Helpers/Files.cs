using System.Diagnostics;
using System.Security.Cryptography;

namespace Launcher.Helpers;

public class FileCategory(string name)
{
    public string Name { get; } = name;
    public List<ManifestFile> List { get; } = [];
}

public static class Files
{
    public static readonly FileCategory Missing = new("missing");
    public static readonly FileCategory Outdated = new("outdated");

    private static async Task<string> GetHashAsync(string filePath)
    {
        using var sha256 = SHA256.Create();
        await using var stream = File.OpenRead(filePath);

        byte[] hashBytes = await sha256.ComputeHashAsync(stream);

        return Convert.ToHexString(hashBytes).ToLower();
    }

    public static async Task Validate(List<ManifestFile> files)
    {
        foreach (var file in files)
        {
            var fullFilePath = Path.Combine(Steam.GamePath, file.Path);

            if (File.Exists(fullFilePath))
            {
                var hash = await GetHashAsync(fullFilePath);
                if (hash == file.Hash)
                    continue;

                Outdated.List.Add(file);
                Terminal.Warning($"Found outdated file: {file.Path}");
            }
            else
            {
                Missing.List.Add(file);
                Terminal.Warning($"Found missing file: {file.Path}");
            }
        }

        if (Missing.List.Count == 0 && Outdated.List.Count == 0)
            Terminal.Success("All files are up-to-date!");
    }

    public static async Task Download(FileCategory category)
    {
        if (category.List.Count == 0)
            return;

        Terminal.Print($"Downloading {category.List.Count} {category.Name} files...");
        foreach (var file in category.List)
        {
            try
            {
                var fullFilePath = Path.Combine(Steam.GamePath, file.Path);
                var directoryPath = Path.GetDirectoryName(fullFilePath);

                if (!string.IsNullOrEmpty(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                var downloadResponse = await Api.Launcher.GetDownload(file.Path);

                await using var fileStream = File.Create(fullFilePath);
                await downloadResponse.CopyToAsync(fileStream);

                Terminal.Success($"Successfully downloaded: {file.Path}");
            }
            catch (Exception e)
            {
                Terminal.Error($"An error occurred while downloading: {file.Path}");
                if (Debugger.IsAttached)
                    Terminal.Debug(e.InnerException?.Message ?? e.Message);
            }
        }
    }
}