using Leopotam.EcsLite;
using MysticEchoes.Core.Loaders;
using MysticEchoes.Core.Scene;

namespace MysticEchoes.Core.AI.Factories;

public class NecromancerFactory : BaseEnemyFactory
{
    public NecromancerFactory(EcsWorld world, EntityBuilder builder, PrefabManager prefabManager) : base(world, builder,
        prefabManager)
    {
    }

    public override int Create(EnemyInitializationInfo enemyInitializationInfo)
    {
        return base.Create(enemyInitializationInfo);
    }
}