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

public class NightBorneFactory : BaseEnemyFactory
{
    protected EcsPool<AiComponent> _ai;
    public NightBorneFactory(EcsWorld world, EntityBuilder builder, ItemsFactory itemsFactory, PrefabManager prefabManager) : base(world, builder, itemsFactory, prefabManager)
    {
        _ai = world.GetPool<AiComponent>();
    }
    
    public override int Create(EnemyInitializationInfo enemyInitializationInfo)
    {
        EnemyInitializationInternalInfo initializationInternalInfo = new EnemyInitializationInternalInfo();
        initializationInternalInfo.EnemyPrefab = PrefabType.NightBorne;
        initializationInternalInfo.EnemyWeaponPrefab = PrefabType.DefaultWeapon;
        int createdEntity = base.CreateInternal(enemyInitializationInfo, initializationInternalInfo);

        ref AiComponent aiComponent = ref _ai.Get(createdEntity);
        aiComponent.BehaviorTree = new NecromancerBt(World, createdEntity);
        aiComponent.BehaviorTree.Start();
        
        ref TransformComponent transformComponent = ref _transforms.Get(createdEntity);
        
        ref DynamicCollider dynamicCollider = ref _colliders.Get(createdEntity);
        dynamicCollider.Box = new Box(createdEntity, new Rectangle(
            Vector2.Zero, 
            new Vector2(0.10f, 0.15f) * transformComponent.Scale
        ));
        
        ref CharacterAnimationComponent enemyAnimations = ref _animations.Get(createdEntity);
        enemyAnimations.AnimationStateMachine = new NightBorneStateMachine(createdEntity, World);
        
        return createdEntity;
    }
}