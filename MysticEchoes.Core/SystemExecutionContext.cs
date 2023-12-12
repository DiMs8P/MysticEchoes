using MysticEchoes.Core.Configuration;
using MysticEchoes.Core.Loaders;

namespace MysticEchoes.Core;

public class SystemExecutionContext
{
    public SystemExecutionContext(ILoader dataLoader)
    {
        Settings = dataLoader.LoadSettings();
    }
    public float DeltaTime { get; set; }
    public Settings Settings { get; set; }
}