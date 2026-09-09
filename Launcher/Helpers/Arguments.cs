namespace Launcher.Helpers;

public static class Arguments
{
    // launcher arguments
    public static bool SkipValidation;
    public static bool DebugEnabled;

    public static List<string> All = [];
    public static List<string> Game = [];

    public static void Initialize()
    {
        var arguments = Environment.GetCommandLineArgs().Skip(1).ToList();

        ReadArgument(ref arguments, "--skip-validation", ref SkipValidation);
        ReadArgument(ref arguments, "--debug", ref DebugEnabled);

        Game = arguments;
    }

    public static void InitializeGame()
    {
        All = [$"--token={GameToken.Value}", "-language harmony", .. Game];
        if (OperatingSystem.IsLinux()) // steam linux runtime thing
            All = ["--", $"\"{Steam.GameExecutable}\"", "-steam", .. All];
    }

    // TODO: make it read other variable types such as int, string, etc.
    private static void ReadArgument(ref List<string> arguments, string value, ref bool argument)
    {
        int index = arguments.IndexOf(value);
        if (index == -1)
            return;

        argument = true;
        arguments.RemoveAt(index);
    }
}