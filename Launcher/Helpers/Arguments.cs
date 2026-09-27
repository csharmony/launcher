namespace Launcher.Helpers;

public static class Arguments
{
    // launcher arguments
    public static bool SkipValidation;
    public static bool DebugEnabled;

    public static List<string> Game = [];

    public static void Initialize()
    {
        var arguments = Environment.GetCommandLineArgs().Skip(1).ToList();

        ReadArgument(ref arguments, "--skip-validation", ref SkipValidation);
        ReadArgument(ref arguments, "--debug", ref DebugEnabled);

        Game = arguments;
    }

    public new static string ToString()
    {
        List<string> arguments = [$"--token={GameToken.Value}", "-language harmony", .. Game];
        if (OperatingSystem.IsLinux()) // steam linux runtime thing
            arguments = ["--", $"\"{Steam.GameExecutable}\"", "-steam", .. arguments];

        return string.Join(" ", arguments);
    }

    // TODO: make it read other variable types such as int, string, etc.
    // ?: i guess overloads is the way, only bool arguments are implemented so no need to add overloads for now
    private static void ReadArgument(ref List<string> arguments, string value, ref bool argument)
    {
        int index = arguments.IndexOf(value);
        if (index == -1)
            return;

        argument = true;
        arguments.RemoveAt(index);
    }
}