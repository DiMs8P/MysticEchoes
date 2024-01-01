using MysticEchoes.Core.Configuration;
using MysticEchoes.Core.Loaders;

namespace MysticEchoes.Core;

public class SystemExecutionContext
{
    public SystemExecutionContext(IDataLoader dataLoader)
    {
        Settings = dataLoader.LoadSettings();
    }
    public int FrameNumber { get; set; }
    public float DeltaTime { get; set; }
    public Settings Settings { get; set; }
}