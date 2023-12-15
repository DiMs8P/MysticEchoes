using System.Numerics;
using System.Windows;

namespace MysticEchoes.Core.Input;

public interface IInputManager
{
    int GetHorizontal();
    int GetVertical();
    bool IsShooting();
    Vector2 GetMousePoint();
}