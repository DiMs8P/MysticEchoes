using System.Numerics;
using Leopotam.EcsLite;
using MysticEchoes.Core.Animations;
using MysticEchoes.Core.Animations.StateMachines;
using MysticEchoes.Core.Base.Geometry;
using MysticEchoes.Core.Collisions;
using MysticEchoes.Core.Collisions.Tree;
using MysticEchoes.Core.Items;
using MysticEchoes.Core.Loaders;
using MysticEchoes.Core.Loaders.Prefabs;
using MysticEchoes.Core.Movement;
using MysticEchoes.Core.Scene;

namespace MysticEchoes.Core.AI.Factories;

public class NecromancerFactory : BaseEnemyFactory
{
    protected EcsPool<AiComponent> _ai;
    public NecromancerFactory(EcsWorld world, EntityBuilder builder, ItemsFactory itemsFactory, PrefabManager prefabManager) : base(world, builder, itemsFactory,
        prefabManager)
    {
        _ai = world.GetPool<AiComponent>();
    }

    public override int Create(EnemyInitializationInfo enemyInitializationInfo)
    {
        EnemyInitializationInternalInfo initializationInternalInfo = new EnemyInitializationInternalInfo();
        initializationInternalInfo.EnemyPrefab = PrefabType.Necromancer;
        initializationInternalInfo.EnemyWeaponPrefab = PrefabType.DefaultWeapon;
        int createdEntity = base.CreateInternal(enemyInitializationInfo, initializationInternalInfo);

        ref AiComponent aiComponent = ref _ai.Get(createdEntity);
        aiComponent.BehaviorTree = new NecromancerBt(World, createdEntity);
        
        ref TransformComponent transformComponent = ref _transforms.Get(createdEntity);
        
        ref DynamicCollider dynamicCollider = ref _colliders.Get(createdEntity);
        dynamicCollider.Box = new Box(createdEntity, new Rectangle(
            Vector2.Zero, 
            new Vector2(0.05f, 0.1f) * transformComponent.Scale
        ));
        
        ref CharacterAnimationComponent enemyAnimations = ref _animations.Get(createdEntity);
        enemyAnimations.AnimationStateMachine = new NecromancerStateMachine(createdEntity, World);
        
        return createdEntity;
    }
}