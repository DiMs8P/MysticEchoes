namespace MysticEchoes.Core.Rendering;

public interface IDispatcher
{
    public void Invoke(Action action);
}