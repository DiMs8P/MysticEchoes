using MysticEchoes.Core.Base.ECS;

namespace MysticEchoes.Core.Rendering;

public class RenderComponent : Component
{
    public RenderingType Type { get; }

    public RenderComponent(RenderingType renderingType)
    {
        Type = renderingType;
    }
}