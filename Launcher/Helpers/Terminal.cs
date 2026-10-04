using Spectre.Console;

namespace Launcher.Helpers;

public static class Terminal
{
    private const string PrimaryColor = "#F58459";
    private const string SecondaryColor = "gray";
    private const string InfoColor = "gray";
    private const string SuccessColor = "green";
    private const string WarningColor = "darkorange";
    private const string ErrorColor = "red";
    private const string DebugColor = "deepskyblue1";

    public static void PrintWelcome()
    {
        AnsiConsole.MarkupLine(
            $"[{PrimaryColor}]Harmony Launcher v{Update.CurrentVersion()}[/] [{SecondaryColor}]made by[/] [{PrimaryColor}]heapy[/]");
    }

    public static void Print(object message)
    {
        AnsiConsole.MarkupLine($"[white on {InfoColor}] ? [/] {Markup.Escape(message.ToString()!)}");
    }

    public static void Success(object message)
    {
        AnsiConsole.MarkupLine(
            $"[white on {SuccessColor}] + [/] [{SuccessColor}]{Markup.Escape(message.ToString()!)}[/]");
    }

    public static void Warning(object message)
    {
        AnsiConsole.MarkupLine(
            $"[white on {WarningColor}] ! [/] [{WarningColor}]{Markup.Escape(message.ToString()!)}[/]");
    }

    public static void Error(object message)
    {
        AnsiConsole.MarkupLine(
            $"[white on {ErrorColor}] X [/] [{ErrorColor}]{Markup.Escape(message.ToString()!)}[/]");
    }

    public static void Debug(object message)
    {
        AnsiConsole.MarkupLine(
            $"[white on {DebugColor}] ? [/] [{DebugColor}]{Markup.Escape(message.ToString()!)}[/]");
    }

    public static void Debug(Exception exception)
    {
        AnsiConsole.MarkupLine(
            $"[{DebugColor}]{Markup.Escape(exception.StackTrace ?? exception.Message)}[/]");
    }

    public static async Task<bool> ConfirmAsync(object message)
    {
        return await AnsiConsole.ConfirmAsync($"[white on {InfoColor}] ? [/] {Markup.Escape(message.ToString()!)}");
    }
}