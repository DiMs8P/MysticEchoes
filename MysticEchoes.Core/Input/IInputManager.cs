namespace MysticEchoes.Core.Input;

public interface IInputManager
{
    int GetHorizontal();
    int GetVertical();
    bool IsShooting();
}