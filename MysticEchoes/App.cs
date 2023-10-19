using System.Threading;
using System.Windows;
using MysticEchoes.Core;
using SharpGL.WPF;

namespace MysticEchoes;

public class App : Application
{
    private readonly MainWindow _mainWindow;
    private readonly Game _game;

    private bool _readyToRender = false;
    private Timer _renderTimer;

    private object gameLock = new object();

    public App(MainWindow mainWindow, Game game)
    {
        _mainWindow = mainWindow;
        _game = game;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        _mainWindow.GlControl.OpenGLInitialized += BindGl;
        _mainWindow.GlControl.Resized += (_, _) => { };
        
        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        var gameTask = RunGame(cancellationToken);
        _mainWindow.Show();

        //base.OnStartup(e);

        // awagameTask.Wait();
        //
        // cts.Cancel();

    }

    private void BindGl(object sender, OpenGLRoutedEventArgs args)
    {
        _mainWindow.GlControl.OpenGLDraw += (_, _) => _game.Render();
         
        _game.Initialize(args.OpenGL, new DispatcherAdapter(_mainWindow.Dispatcher));
    }

    public async Task RunGame(CancellationToken cancellationToken)
    {
        _renderTimer = new Timer(RenderTimerCallback!, null, 0, 20);

        // await Task.Run(() =>
        // {
        //     GameLoop(cancellationToken);
        // });
    }

    private void RenderTimerCallback(object _)
    {
        if (!_readyToRender) return;

        lock (gameLock)
        {
            _readyToRender = false;

            _game.Update();

            _mainWindow.Dispatcher.Invoke(_mainWindow.GlControl.DoRender);
        }

        _readyToRender = true;
    }

    private void GameLoop(CancellationToken cancellationToken)
    {
        while (true)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _renderTimer.Dispose();
                return;
            }
            _game.Update();
            _readyToRender = true;
        }
    }
}