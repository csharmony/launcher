using System.Diagnostics;

namespace Launcher.Helpers;

public static class Debug
{
    public static bool IsEnabled => Debugger.IsAttached || Arguments.DebugEnabled;
}