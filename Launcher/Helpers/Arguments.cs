namespace Launcher.Helpers;

public static class Arguments
{
    public static bool SkipValidation;
    public static string All = "";
    public static string Game = "";

    public static void Initialize()
    {
        var arguments = string.Join(" ", Environment.GetCommandLineArgs().Skip(1));

        if (arguments.Contains("--skip-validation"))
        {
            SkipValidation = true;
            arguments = arguments.Replace("--skip-validation", "");
        }

        Game = arguments;

        All = $"--token={GameToken.Value} {arguments}";
        if (OperatingSystem.IsLinux()) // steam linux runtime thing
            All = $"-- \"{Steam.GameExecutable}\" -steam " + All;
    }
}