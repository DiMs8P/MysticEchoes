using MysticEchoes.Core.Base;
using SharpGL;

namespace MysticEchoes.Core.Rendering;

public abstract class RenderComponent : LifeCycleComponent
{
    protected OpenGL GL { get; private set; } = null!;

    public void InitGl(OpenGL gl)
    {
        GL = gl;
    }

    public abstract override void Update();
}