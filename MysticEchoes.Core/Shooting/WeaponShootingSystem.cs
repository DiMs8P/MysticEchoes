using System.Drawing;
using System.Numerics;
using Leopotam.EcsLite;
using MysticEchoes.Core.Configuration;
using MysticEchoes.Core.Input;
using MysticEchoes.Core.Loaders;
using MysticEchoes.Core.Loaders.Prefabs;
using MysticEchoes.Core.Movement;
using MysticEchoes.Core.Scene;
using SevenBoldPencil.EasyDi;
using SharpGL;
using SharpGL.SceneGraph;

namespace MysticEchoes.Core.Shooting;

public class WeaponShootingSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] private SystemExecutionContext _systemExecutionContext;
    [EcsInject] private PrefabManager _prefabManager;
    [EcsInject] private EntityFactory _factory;
    [EcsInject] private IInputManager _inputManager;
    [EcsInject] private OpenGL _gl;

    private EcsFilter _weaponsFilter;
    private EcsPool<TransformComponent> _transforms;
    private EcsPool<MovementComponent> _movements;
    private EcsPool<WeaponComponent> _weapons;
    private EcsPool<DamageComponent> _damages;

    private WeaponsSettings _weaponsSettings;

    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _weaponsFilter = world.Filter<WeaponComponent>().Inc<TransformComponent>().End();

        _transforms = world.GetPool<TransformComponent>();
        _movements = world.GetPool<MovementComponent>();
        _weapons = world.GetPool<WeaponComponent>();
        _damages = world.GetPool<DamageComponent>();
        _weaponsSettings = _systemExecutionContext.Settings.WeaponsSettings;
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var entityId in _weaponsFilter)
        {
            ref WeaponComponent weaponComponent = ref _weapons.Get(entityId);

            if (weaponComponent.State == WeaponState.Shooting)
            {
                MakeShot(entityId);
                weaponComponent.ElapsedTimeFromLastShoot = 0;
                weaponComponent.State = WeaponState.ReadyToFire;
                continue;
            }

            weaponComponent.ElapsedTimeFromLastShoot += _systemExecutionContext.DeltaTime;
        }
    }

    private void MakeShot(int entityId)
    {
        ref WeaponComponent weapon = ref _weapons.Get(entityId);

        if (weapon.Type is WeaponType.OneShoot)
        {
            ref TransformComponent transformComponent = ref _transforms.Get(entityId);

            SpawnProjectile(_factory,
                weapon.ProjectilePrefab,
                transformComponent.Location,
                transformComponent.Rotation,
                _weaponsSettings.OneShot.Damage,
                _weaponsSettings.OneShot.BulletSpeed);
        }
        else if (weapon.Type is WeaponType.TwoShoot)
        {
            ref TransformComponent transformComponent = ref _transforms.Get(entityId);

            SpawnProjectile(_factory,
                weapon.ProjectilePrefab,
                transformComponent.Location + transformComponent.Rotation.Inverse().ReflectY() * _weaponsSettings.Twoshot.DistanceBetweenBullets / 2,
                transformComponent.Rotation,
                _weaponsSettings.Twoshot.Damage,
                _weaponsSettings.Twoshot.BulletSpeed);
            SpawnProjectile(_factory,
                weapon.ProjectilePrefab,
                transformComponent.Location + transformComponent.Rotation.Inverse().ReflectY() * _weaponsSettings.Twoshot.DistanceBetweenBullets / 2 * (-1),
                transformComponent.Rotation,
                _weaponsSettings.Twoshot.Damage,
                _weaponsSettings.Twoshot.BulletSpeed);
        }
        else if (weapon.Type is not WeaponType.None)
        {
            throw new NotImplementedException();
        }
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