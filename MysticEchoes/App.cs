using MysticEchoes.Core;
using SharpGL.WPF;
using System.Windows;

namespace MysticEchoes;

public class App : Application
{
    private readonly MainWindow _mainWindow;
    private readonly Game _game;

    private bool _readyToRender = false;
    private Timer _renderTimer;

    private readonly object gameLock = new object();

    public App(MainWindow mainWindow, Game game)
    {
        _mainWindow = mainWindow;
        _game = game;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        _mainWindow.GlControl.OpenGLInitialized += BindGl;
        _mainWindow.GlControl.Resized += (_, _) => { };

        RunGame();
        _mainWindow.Show();
        // base.OnStartup(e);
    }

    private void BindGl(object sender, OpenGLRoutedEventArgs args)
    {
        _mainWindow.GlControl.OpenGLDraw += (_, _) => _game.Render();

        _game.Initialize(args.OpenGL);
    }

    public void RunGame()
    {
        _renderTimer = new Timer(RenderTimerCallback!, null, 0, 20);
    }

    private void RenderTimerCallback(object _)
    {
        if (!_readyToRender) return;

        _readyToRender = false;

        _game.Update();

        _mainWindow.Dispatcher.Invoke(_mainWindow.GlControl.DoRender);

        _readyToRender = true;
    }
}