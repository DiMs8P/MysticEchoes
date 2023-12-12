namespace MysticEchoes.Core.Input;

public interface IInputManager
{
    float GetHorizontal();
    float GetVertical();
    bool IsShooting();
}