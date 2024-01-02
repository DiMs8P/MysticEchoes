using MysticEchoes.Core.Loaders.Prefabs;

namespace MysticEchoes.Core.Shooting;

public struct ExplosionComponent
{
    public PrefabType ExplosionPrefab { get; set; }

    public ExplosionComponent()
    {
        ExplosionPrefab = PrefabType.None;
    }
}