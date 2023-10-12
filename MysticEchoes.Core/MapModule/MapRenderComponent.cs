using MysticEchoes.Core.Base;
using MysticEchoes.Core.Rendering;

namespace MysticEchoes.Core.MapModule;

public class MapRenderComponent : RenderComponent
{
    private Map _map = null!;

    public override void Initialize(Entity entity)
    {
        base.Initialize(entity);
        _map = entity.GetComponent<Map>();
    }

    public override void Update()
    {
        GL.Vertex(0d, 0d);
        throw new NotImplementedException();
    }
}