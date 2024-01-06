using System.Numerics;

namespace MysticEchoes.Core.Config.Input;

public interface IInputManager
{
    int GetHorizontal();
    int GetVertical();
    bool IsShooting();
    Vector2 GetMousePosition();
}