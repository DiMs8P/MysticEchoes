using Leopotam.EcsLite;
using MysticEchoes.Core.Loaders;
using MysticEchoes.Core.Scene;

namespace MysticEchoes.Core.AI.Factories;

public class NecromancerFactory : BaseEnemyFactory
{
    public NecromancerFactory(EcsWorld world, EntityBuilder builder, PrefabManager prefabManager, EnemyInfo enemyInfo) : base(world, builder, prefabManager, enemyInfo)
    {
    }

    public override int Create()
    {
        return base.Create();
    }
}