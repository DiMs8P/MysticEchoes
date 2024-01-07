using System.Numerics;
using Leopotam.EcsLite;
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
    public NecromancerFactory(EcsWorld world, EntityBuilder builder, ItemsFactory itemsFactory, PrefabManager prefabManager) : base(world, builder, itemsFactory,
        prefabManager)
    {
    }

    public override int Create(EnemyInitializationInfo enemyInitializationInfo)
    {
        EnemyInitializationInternalInfo initializationInternalInfo = new EnemyInitializationInternalInfo()
        {
            EnemyPrefab = PrefabType.Necromancer,
            EnemyWeaponPrefab = PrefabType.DefaultWeapon,
            EnemyBehaviorTree = typeof(NecromancerBt),
            EnemyStateMachine = typeof(IdleRunShootingHitDeathStateMachine)
        };
        
        int createdEntity = base.CreateInternal(enemyInitializationInfo, initializationInternalInfo);

        ref TransformComponent transformComponent = ref _transforms.Get(createdEntity);
        ref DynamicCollider dynamicCollider = ref _colliders.Get(createdEntity);
        dynamicCollider.Box = new Box(createdEntity, new Rectangle(
            Vector2.Zero, 
            new Vector2(0.05f, 0.1f) * transformComponent.Scale
        ));
        
        return createdEntity;
    }
}