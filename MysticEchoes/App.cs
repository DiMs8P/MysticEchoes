using MysticEchoes.Core;
using SharpGL.WPF;
using System.Windows;
using MysticEchoes.Implementations;

namespace MysticEchoes;

public class App : Application
{
    private readonly MainWindow _mainWindow;
    private readonly Game _game;
    
    private BaseInputManager _inputManager;

    private bool _readyToRender = false;
    private Timer _renderTimer;

    private readonly object gameLock = new object();
    public App(MainWindow mainWindow, Game game)
    {
        _mainWindow = mainWindow;
        _game = game;

        _inputManager = (BaseInputManager?)_game.InputManager;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        _mainWindow.GlControl.OpenGLInitialized += InitializeGame;
        _mainWindow.GlControl.Resized += (_, _) => { };
        
        _mainWindow.Show();
        RunGame();
        // base.OnStartup(e);
    }

    private void InitializeGame(object sender, OpenGLRoutedEventArgs args)
    {
        /*_inputManager = new BaseInputManager();

        Environment environment = new Environment(args.OpenGL, _inputManager, new MazeGeneratorAdapter());
        Settings settings = new Settings();
        
        _game = new Game(environment, settings);*/
        _game.InitializeRender(args.OpenGL);
        _mainWindow.GlControl.OpenGLDraw += (_, _) =>
        {
            _game.Render();
        };
    }

    public void RunGame()
    {
        _readyToRender = true;
        // Todo попробовать добавить в state mainWindow, game и readyToRender
        // Возможно это уменьшит количество сборок мусора
        _renderTimer = new Timer(RenderTimerCallback!, null, 0, 40);
    }

    private void RenderTimerCallback(object _)
    {
        if (!_readyToRender) return;

        lock (gameLock)
        {
            _readyToRender = false;

            _inputManager.SetMousePosition(_mainWindow.mousePosition);
            // TODO think about it
            _mainWindow.Dispatcher.Invoke(_inputManager.Update);
            _mainWindow.Dispatcher.Invoke(_game.Update);
            try
            {
                _mainWindow.Dispatcher.Invoke(_mainWindow.GlControl.DoRender);
            }
            catch (TaskCanceledException)
            {
                return;
            }

            _readyToRender = true;
        }
    }
    
    protected override void OnExit(ExitEventArgs e)
    {
        _renderTimer.Dispose();
        
        base.OnExit(e);
    }
}