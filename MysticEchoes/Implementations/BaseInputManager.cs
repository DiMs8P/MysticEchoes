using System.Windows.Input;
using MysticEchoes.Core.Config.Input;

namespace MysticEchoes.Implementations;

public class BaseInputManager : IInputManager
{
    private int _horizontal = 0;
    private int _vertical = 0;
    private bool _shooting = false;
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

    public void Update()
    {
        _horizontal = 0;
        _vertical = 0;
        _shooting = Mouse.LeftButton == MouseButtonState.Pressed;
        
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