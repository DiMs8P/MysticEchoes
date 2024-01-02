using MysticEchoes.Core.Loaders.Prefabs;

namespace MysticEchoes.Core.Shooting;

public struct MagicComponent
{
    public MagicType Type { get; set; }

    public MagicComponent()
    {
        Type = MagicType.None;
    }
}