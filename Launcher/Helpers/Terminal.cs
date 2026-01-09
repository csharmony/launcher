using Spectre.Console;

namespace Launcher.Helpers;

public static class Terminal
{
    private const string PrimaryColor = "DeepPink1_1";
    private const string SecondaryColor = "gray";
    private const string InfoColor = "gray";
    private const string SuccessColor = "green";
    private const string WarningColor = "darkorange";
    private const string ErrorColor = "red";
    private const string DebugColor = "deepskyblue1";

    private const string Prefix = $"[{SecondaryColor}][[[/]" + $"[{PrimaryColor}]Launcher[/]" +
                                  $"[{SecondaryColor}]]][/]";

    public static void PrintWelcome()
    {
        AnsiConsole.MarkupLine(
            $"[{PrimaryColor}]Launcher[/] [black on {PrimaryColor}] ALPHA [/] [{SecondaryColor}]made by[/] [black on {PrimaryColor}] heapy [/]");
    }

    public static void Print(object message)
    {
        AnsiConsole.MarkupLine($"{Prefix} [white on {InfoColor}] ? [/] {Markup.Escape(message.ToString())}");
    }

    public static void Success(object message)
    {
        AnsiConsole.MarkupLine(
            $"{Prefix} [white on {SuccessColor}] ✓ [/] [{SuccessColor}]{Markup.Escape(message.ToString())}[/]");
    }

    public static void Warning(object message)
    {
        AnsiConsole.MarkupLine(
            $"{Prefix} [white on {WarningColor}] ! [/] [{WarningColor}]{Markup.Escape(message.ToString())}[/]");
    }

    public static void Error(object message)
    {
        AnsiConsole.MarkupLine(
            $"{Prefix} [white on {ErrorColor}] X [/] [{ErrorColor}]{Markup.Escape(message.ToString())}[/]");
    }

    public static void Debug(object message)
    {
        AnsiConsole.MarkupLine(
            $"{Prefix} [white on {DebugColor}] ? [/] [{DebugColor}]{Markup.Escape(message.ToString())}[/]");
    }
}