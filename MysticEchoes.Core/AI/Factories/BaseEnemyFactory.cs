using Leopotam.EcsLite;
using MysticEchoes.Core.Loaders;
using MysticEchoes.Core.Scene;

namespace MysticEchoes.Core.AI.Factories;

public class BaseEnemyFactory : IEnemyFactory
{
    protected EcsWorld World;
    protected EntityBuilder Builder;
    protected PrefabManager PrefabManager;
    
    public BaseEnemyFactory(EcsWorld world, EntityBuilder builder, PrefabManager prefabManager)
    {
        World = world;
        Builder = builder;
        PrefabManager = prefabManager;
    }

    public virtual int Create(EnemyInitializationInfo enemyInitializationInfo)
    {
        int createdEnemy = PrefabManager.CreateEntityFromPrefab(Builder, enemyInitializationInfo.Prefab);

        return createdEnemy;
    }
}