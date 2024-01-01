using MysticEchoes.Core.Loaders.Prefabs;

namespace MysticEchoes.Core.Shooting;

public struct MagicComponent
{
    public PrefabType AmmoPrefab { get; set; }
    public AmmoType Type { get; set; }

    public MagicComponent()
    {
        AmmoPrefab = PrefabType.None;
        Type = AmmoType.None;
    }
}