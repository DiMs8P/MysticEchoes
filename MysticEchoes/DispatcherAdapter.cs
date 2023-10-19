using System.Windows.Threading;
using MysticEchoes.Core.Rendering;

namespace MysticEchoes;

public class DispatcherAdapter : IDispatcher
{
    private readonly Dispatcher _dispatcher;

    public DispatcherAdapter(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public void Invoke(Action action)
    {
        _dispatcher.Invoke(action);
    }
}