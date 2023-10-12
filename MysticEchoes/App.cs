using System.Windows;
using MysticEchoes.Core;

namespace MysticEchoes;

public class App : Application
{
    readonly MainWindow _mainWindow;

    public App(MainWindow mainWindow, Game game)
    {
        this._mainWindow = mainWindow;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        _mainWindow.Show();
        base.OnStartup(e);
    }
}