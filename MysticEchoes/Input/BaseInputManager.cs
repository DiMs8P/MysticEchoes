using System.Windows.Input;
using MysticEchoes.Core.Input;

namespace MysticEchoes.Input;

public class BaseInputManager : IInputManager
{
    private float _horizontal;
    private float _vertical;
    private bool _shooting;
    public float GetHorizontal()
    {
        return _horizontal;
    }

    public float GetVertical()
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