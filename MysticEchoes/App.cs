using MysticEchoes.Core;
using SharpGL.WPF;
using System.Windows;
using System.Windows.Input;

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

        LoadGameSettings();

        _mainWindow.Show();
        RunGame();
        // base.OnStartup(e);
    }

    private void LoadGameSettings()
    {
    }

    private void BindGl(object sender, OpenGLRoutedEventArgs args)
    {
        _mainWindow.GlControl.OpenGLDraw += (_, _) => _game.Render();

        _game.Initialize(args.OpenGL);
    }

    public void RunGame()
    {
        _readyToRender = true;
        // Todo попробовать добавить в state mainWindow, game и readyToRender
        // Возможно это уменьшит количество сборок мусора
        _renderTimer = new Timer(RenderTimerCallback!, null, 0, 20);
    }

    private void RenderTimerCallback(object _)
    {
        if (!_readyToRender) return;

        lock (gameLock)
        {
            _readyToRender = false;

            // TODO think about it
            _mainWindow.Dispatcher.Invoke(ProcessPlayerInput);
            _game.Update();
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

    private void ProcessPlayerInput()
    {
        _game.InputManager.Horizontal = 0;
        _game.InputManager.Vertical = 0;
        _game.InputManager.Shooting = Mouse.LeftButton == MouseButtonState.Pressed;
        
        if (Keyboard.IsKeyDown(Key.W))
        {
            _game.InputManager.Vertical += 1;
        }
        
        if (Keyboard.IsKeyDown(Key.S))
        {
            _game.InputManager.Vertical -= 1;
        }
        
        if (Keyboard.IsKeyDown(Key.A))
        {
            _game.InputManager.Horizontal -= 1;
        }
        
        if (Keyboard.IsKeyDown(Key.D))
        {
            _game.InputManager.Horizontal += 1;
        }
    }
}