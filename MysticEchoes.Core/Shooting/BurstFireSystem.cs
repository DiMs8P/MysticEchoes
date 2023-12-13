using Leopotam.EcsLite;

namespace MysticEchoes.Core.Shooting;

public class BurstFireSystem : IEcsInitSystem, IEcsRunSystem
{
    private EcsFilter _burstWeaponsFilter;
    private EcsPool<WeaponComponent> _weapons;
    private EcsPool<BurstFireComponent> _burstWeapons;

    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _burstWeaponsFilter = world.Filter<WeaponComponent>().Inc<BurstFireComponent>().End();
        
        _weapons = world.GetPool<WeaponComponent>();
        _burstWeapons = world.GetPool<BurstFireComponent>();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var entityId in _burstWeaponsFilter)
        {
            ref WeaponComponent weaponComponent = ref _weapons.Get(entityId);

            if (weaponComponent.State == WeaponState.Shooting)
            {
                ref BurstFireComponent burstWeaponComponent = ref _burstWeapons.Get(entityId);

                if (burstWeaponComponent.FiredShots + 1 >= burstWeaponComponent.MaxShotsInBurst)
                {
                    weaponComponent.TimeBetweenShots = burstWeaponComponent.TimeBetweenBursts;
                    burstWeaponComponent.FiredShots = 0;
                }
                else
                {
                    weaponComponent.TimeBetweenShots = burstWeaponComponent.TimeBetweenBurstShots;
                    ++burstWeaponComponent.FiredShots;
                }
            }
        }
    }
}