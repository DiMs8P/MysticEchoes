using System.Numerics;
using System.Windows.Input;
using MysticEchoes.Core.Input;

namespace MysticEchoes.Implementations;

public class BaseInputManager : IInputManager
{
    private int _horizontal = 0;
    private int _vertical = 0;
    private bool _shooting = false;
    Vector2 _mousePosition;
    public int GetHorizontal()
    {
        return _horizontal;
    }

    public int GetVertical()
    {
        return _vertical;
    }

    public bool IsShooting()
    {
        return _shooting;
    }

    public Vector2 GetMousePoint()
    {
        return _mousePosition;
    }

    private Vector2 CalcMousePoint(MainWindow mainWindow)
    {
        _mousePosition = mainWindow._mousePosition;
        _mousePosition.X = 2.0f * _mousePosition.X / (float)mainWindow.ActualWidth;
        _mousePosition.Y = 2.0f * ((float)mainWindow.ActualHeight - _mousePosition.Y) / (float)mainWindow.ActualHeight;
        return _mousePosition;
    }

    public void Update(MainWindow mainWindow)
    {
        _horizontal = 0;
        _vertical = 0;
        _shooting = Mouse.LeftButton == MouseButtonState.Pressed;
        _mousePosition = CalcMousePoint(mainWindow);

        if (Keyboard.IsKeyDown(Key.W))
        {
            _vertical += 1;
        }

        if (Keyboard.IsKeyDown(Key.S))
        {
            _vertical -= 1;
        }

        if (Keyboard.IsKeyDown(Key.A))
        {
            _horizontal -= 1;
        }

        if (Keyboard.IsKeyDown(Key.D))
        {
            _horizontal += 1;
        }
    }
}