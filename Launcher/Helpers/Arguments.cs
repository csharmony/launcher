namespace Launcher.Helpers;

public static class Arguments
{
    // launcher arguments
    public static bool SkipValidation;
    public static bool DebugEnabled;
    public static bool SkipUpdates;

    public static List<string> List = [];

    public static void Initialize()
    {
        var arguments = Environment.GetCommandLineArgs().Skip(1).ToList();

        ReadArgument(ref arguments, "--skip-validation", ref SkipValidation);
        ReadArgument(ref arguments, "--debug", ref DebugEnabled);
        ReadArgument(ref arguments, "--skip-updates", ref SkipUpdates);

        List = ["-language harmony", .. arguments];
        if (OperatingSystem.IsLinux()) // steam linux runtime thing
            List = ["--", $"\"./{Steam.GameExecutable}\"", "-steam", .. List];
    }

    public new static string ToString() => string.Join(" ", List);

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