namespace Launcher.Helpers;

public static class Arguments
{
    // launcher arguments
    public static bool SkipValidation;
    public static bool DebugEnabled;

    public static string All = "";
    public static string Game = "";

    public static void InitializeLauncher()
    {
        var arguments = string.Join(" ", Environment.GetCommandLineArgs().Skip(1));

        ReadArgument(ref arguments, "--skip-validation", ref SkipValidation);
        ReadArgument(ref arguments, "--debug", ref DebugEnabled);

        Game = arguments;
    }

    public static void InitializeGame()
    {
        All = $"--token={GameToken.Value} {Game}";
        if (OperatingSystem.IsLinux()) // steam linux runtime thing
            All = $"-- \"{Steam.GameExecutable}\" -steam " + All;
    }

    // TODO: make it read other variable types such as int, string, etc.
    private static void ReadArgument(ref string arguments, string value, ref bool argument)
    {
        if (!arguments.Contains(value))
            return;

        argument = true;
        arguments = arguments.Replace(value, "");
    }
}