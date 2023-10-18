namespace MysticEchoes.Core.Rendering;

public class RenderPool
{
    private readonly SortedDictionary<int, List<RenderStrategy>> _renderStrategyByLayer = new();

    public void Add(RenderStrategy strategy)
    {
        if (!_renderStrategyByLayer.TryGetValue(strategy.Layer, out List<RenderStrategy>? value))
        {
            value = new List<RenderStrategy>();
            _renderStrategyByLayer.Add(strategy.Layer, value);
        }

        value.Add(strategy);
    }

    public IEnumerable<RenderStrategy> Enumerate()
    {
        return _renderStrategyByLayer.SelectMany(kv => kv.Value);
    }
}