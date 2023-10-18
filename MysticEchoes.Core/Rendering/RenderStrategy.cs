using SharpGL;
// ReSharper disable InconsistentNaming

namespace MysticEchoes.Core.Rendering;

public abstract class RenderStrategy
{
    protected OpenGL GL { get; private set; } = null!;
    public virtual int Layer { get; }

    public void InitGl(OpenGL gl)
    {
        GL = gl;
    }

    public abstract void DoRender();
}