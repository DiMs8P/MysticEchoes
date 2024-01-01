using System.Numerics;
using Leopotam.EcsLite;
using MysticEchoes.Core.Configuration;
using MysticEchoes.Core.Loaders;
using MysticEchoes.Core.Loaders.Prefabs;
using MysticEchoes.Core.Movement;
using MysticEchoes.Core.Scene;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Shooting;

public class WeaponShootingSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] private SystemExecutionContext _systemExecutionContext;
    [EcsInject] private PrefabManager _prefabManager;
    [EcsInject] private EntityFactory _factory;

    private EcsFilter _weaponOwnersFilter;
    private EcsPool<TransformComponent> _transforms;
    private EcsPool<MovementComponent> _movements;
    private EcsPool<WeaponOwnerComponent> _weapons;
    private EcsPool<MuzzleComponent> _muzzles;
    private EcsPool<ProjectileComponent> _projectiles;
    private EcsPool<DamageComponent> _damages;

    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _weaponOwnersFilter = world.Filter<WeaponOwnerComponent>().Inc<TransformComponent>().End();

        _transforms = world.GetPool<TransformComponent>();
        _movements = world.GetPool<MovementComponent>();
        _weapons = world.GetPool<WeaponOwnerComponent>();
        _damages = world.GetPool<DamageComponent>();
        _muzzles = world.GetPool<MuzzleComponent>();
        _projectiles = world.GetPool<ProjectileComponent>();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var weaponOwnerId in _weaponOwnersFilter)
        {
            ref WeaponOwnerComponent weaponOwnerComponent = ref _weapons.Get(weaponOwnerId);


            foreach (var weaponEntityId in weaponOwnerComponent.WeaponIds)
            {
                ref MuzzleComponent muzzleComponent = ref _muzzles.Get(weaponEntityId);

                if (weaponOwnerComponent.IsShooting)
                {
                    if (muzzleComponent.ShootingType == ShootingType.SingleShot)
                    {
                        if (muzzleComponent.ElapsedTimeFromLastShot > muzzleComponent.TimeBetweenShots)
                        {
                            MakeShot(weaponOwnerId, weaponEntityId);
                            muzzleComponent.ElapsedTimeFromLastShot = 0;
                            continue;
                        }
                    }
                }
                
                muzzleComponent.ElapsedTimeFromLastShot += _systemExecutionContext.DeltaTime;
            }
        }
    }

    private void MakeShot(int ownerEntityId, int weaponEntityId)
    {
        ref ProjectileComponent projectileComponent = ref _projectiles.Get(weaponEntityId);
        ref TransformComponent weaponLocalTransformComponent = ref _transforms.Get(weaponEntityId);
        ref TransformComponent ownerTransformComponent = ref _transforms.Get(ownerEntityId);

        SpawnProjectile(_factory,
            projectileComponent.ProjectilePrefab,
            ownerTransformComponent.Location + weaponLocalTransformComponent.Location,
            ownerTransformComponent.Rotation + weaponLocalTransformComponent.Rotation,
            5,
            0.2f);
    }
    
    private int SpawnProjectile(EntityFactory factory, PrefabType projectileId, Vector2 projectileLocation, Vector2 projectileRotation, float damage, float speed)
    {
        int projectile = _prefabManager.CreateEntityFromPrefab(factory, projectileId);

        ref TransformComponent projectileTransformComponent = ref _transforms.Get(projectile);
        projectileTransformComponent.Location = projectileLocation;
        projectileTransformComponent.Rotation = projectileRotation;
        
        ref MovementComponent projectileMovementComponent = ref _movements.Get(projectile);
        projectileMovementComponent.Velocity = projectileRotation;
        projectileMovementComponent.Speed = speed;
        
        ref DamageComponent projectileDamageComponent = ref _damages.Get(projectile);
        projectileDamageComponent.Damage = damage;
        
        return projectile;
    }
}