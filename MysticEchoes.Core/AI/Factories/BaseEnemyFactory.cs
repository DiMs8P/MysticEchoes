using System.Numerics;
using Leopotam.EcsLite;
using MysticEchoes.Core.Animations;
using MysticEchoes.Core.Animations.StateMachines;
using MysticEchoes.Core.Base.Geometry;
using MysticEchoes.Core.Collisions;
using MysticEchoes.Core.Collisions.Tree;
using MysticEchoes.Core.Loaders;
using MysticEchoes.Core.Movement;
using MysticEchoes.Core.Scene;

namespace MysticEchoes.Core.AI.Factories;

public class BaseEnemyFactory : IEnemyFactory
{
    protected EcsWorld World;
    protected EntityBuilder Builder;
    protected PrefabManager PrefabManager;

    protected EcsPool<TransformComponent> _transforms;
    protected EcsPool<DynamicCollider> _colliders;
    protected EcsPool<CharacterAnimationComponent> _animations;
    
    public BaseEnemyFactory(EcsWorld world, EntityBuilder builder, PrefabManager prefabManager)
    {
        World = world;
        Builder = builder;
        PrefabManager = prefabManager;

        _transforms = world.GetPool<TransformComponent>();
        _colliders = world.GetPool<DynamicCollider>();
        _animations = world.GetPool<CharacterAnimationComponent>();
    }

    public virtual int Create(EnemyInitializationInfo enemyInitializationInfo)
    {
        int createdEnemy = PrefabManager.CreateEntityFromPrefab(Builder, enemyInitializationInfo.Prefab);

        ref TransformComponent transformComponent = ref _transforms.Get(createdEnemy);
        transformComponent.Location = enemyInitializationInfo.Location;
        
        ref DynamicCollider dynamicCollider = ref _colliders.Get(createdEnemy);
        dynamicCollider = new DynamicCollider()
        {
            Box = new Box(createdEnemy, new Rectangle(
                Vector2.Zero,
                new Vector2(0.265f, 0.35f) * transformComponent.Scale
            ))
        };
        dynamicCollider.Behavior = CollisionBehavior.EnemyCharacter;
        
        ref CharacterAnimationComponent enemyAnimations = ref _animations.Get(createdEnemy);
        enemyAnimations.AnimationStateMachine = new CharacterStateMachine(createdEnemy, World);
        
        return createdEnemy;
    }
}