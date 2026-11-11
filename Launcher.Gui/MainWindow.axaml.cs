using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace Launcher.Gui;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    // clicking empty space should drop focus on elements
    private void OnWindowPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.Source is Visual source &&
            source.GetSelfAndVisualAncestors().OfType<InputElement>().Any(x => x.Focusable))
            return;

        FocusManager?.Focus(null);
    }
}