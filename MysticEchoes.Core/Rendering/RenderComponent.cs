namespace MysticEchoes.Core.Rendering;

public struct RenderComponent
{
    public RenderingType Type { get; }

    public RenderComponent(RenderingType renderingType)
    {
        Type = renderingType;
    }
}