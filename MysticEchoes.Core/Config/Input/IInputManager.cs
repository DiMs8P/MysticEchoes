namespace MysticEchoes.Core.Config.Input;

public interface IInputManager
{
    int GetHorizontal();
    int GetVertical();
    bool IsShooting();
}