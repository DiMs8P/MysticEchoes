using Leopotam.EcsLite;
using MysticEchoes.Core.Loaders;
using MysticEchoes.Core.Loaders.Prefabs;
using MysticEchoes.Core.Scene;

namespace MysticEchoes.Core.AI.Factories;

public class NecromancerFactory : BaseEnemyFactory
{
    protected EcsPool<AiComponent> _ai;
    public NecromancerFactory(EcsWorld world, EntityBuilder builder, PrefabManager prefabManager) : base(world, builder,
        prefabManager)
    {
        _ai = world.GetPool<AiComponent>();
    }

    public override int Create(EnemyInitializationInfo enemyInitializationInfo)
    {
        enemyInitializationInfo.Prefab = PrefabType.Necromancer;
        int createdEntity = base.Create(enemyInitializationInfo);

        ref AiComponent aiComponent = ref _ai.Get(createdEntity);
        aiComponent.BehaviorTree = new NecromancerBt(World, createdEntity);
        
        return createdEntity;
    }
}