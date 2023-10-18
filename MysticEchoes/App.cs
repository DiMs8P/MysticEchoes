using System.Windows;
using MysticEchoes.Core;

namespace MysticEchoes;

public class App : Application
{
    private readonly MainWindow _mainWindow;
    private readonly Game _game;

    public App(MainWindow mainWindow, Game game)
    {
        _mainWindow = mainWindow;
        _game = game;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        // _game.Initialize();
        _mainWindow.Show();

        base.OnStartup(e);
    }
}