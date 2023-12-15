using MysticEchoes.Core.Configuration;
using MysticEchoes.Core.Loaders;
using SharpGL.SceneGraph;


namespace MysticEchoes.Core;

public class SystemExecutionContext
{
    public SystemExecutionContext(IDataLoader dataLoader)
    {
        Settings = dataLoader.LoadSettings();
    }
    public float DeltaTime { get; set; }
    public Settings Settings { get; set; }
    public Matrix MatrixView { get; set; }
    public Matrix MatrixProjection { get; set; }
}