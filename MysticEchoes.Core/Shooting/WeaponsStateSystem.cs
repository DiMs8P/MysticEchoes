using Leopotam.EcsLite;

namespace MysticEchoes.Core.Shooting;

public class WeaponsStateSystem : IEcsInitSystem, IEcsRunSystem
{
    private EcsFilter _weaponsFilter;
    private EcsPool<WeaponComponent> _weapons;
    
    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _weaponsFilter = world.Filter<WeaponComponent>().End();
        _weapons = world.GetPool<WeaponComponent>();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var entityId in _weaponsFilter)
        {
            ref WeaponComponent weaponComponent = ref _weapons.Get(entityId);

            if (weaponComponent.State == WeaponState.WantsToFire)
            {
                if (weaponComponent.ElapsedTimeFromLastShoot >= weaponComponent.TimeBetweenShots) 
                {
                    weaponComponent.State = WeaponState.Shooting;
                }
            }
        }
    }
}