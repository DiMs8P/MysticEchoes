using MysticEchoes.Core.Loaders.Prefabs;

namespace MysticEchoes.Core.Shooting;

public struct ProjectileComponent
{
    public PrefabType ProjectilePrefab { get; set; }

    public ProjectileComponent()
    {
        ProjectilePrefab = PrefabType.None;
    }
}