using MysticEchoes.Core.Loaders.Prefabs;

namespace MysticEchoes.Core.Shooting;

public struct MagicComponent
{
    public PrefabType MagicPrefab { get; set; }
    public MagicType Type { get; set; }

    public MagicComponent()
    {
        MagicPrefab = PrefabType.None;
        Type = MagicType.None;
    }
}