namespace MysticEchoes.Core.Rendering;

public class Renderer
{
    private readonly RenderPool _pool;

    public Renderer(RenderPool pool)
    {
        _pool = pool;
    }

    public void DoRender()
    {
        foreach (var renderStrategy in _pool.Enumerate())
        {
            renderStrategy.DoRender();
        }
        _pool.Clear();
    }

    public void AddInPool(RenderStrategy strategy)
    {
        _pool.Add(strategy);
    }
}