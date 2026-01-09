using System.Diagnostics;
using Launcher.Helpers;

Terminal.PrintWelcome();

if (Debugger.IsAttached)
{
    Terminal.Print("This is an info message");
    Terminal.Success("This is a success message");
    Terminal.Warning("This is a warning message");
    Terminal.Error("This is an error message");
    Terminal.Debug("This is a debug message");
}