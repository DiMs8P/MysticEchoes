using Leopotam.EcsLite;
using MysticEchoes.Core.Config.Input;
using MysticEchoes.Core.Control;
using MysticEchoes.Core.Player;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Shooting;

public class PlayerShootingSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] private IInputManager _inputManager;
    
    private EcsFilter _playerFilter;
    private EcsPool<WeaponComponent> _weapons;
    private EcsPool<UnitControlComponent> _controls;

    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _playerFilter = world.Filter<PlayerMarker>().Inc<WeaponComponent>().End();
        if (_playerFilter.GetEntitiesCount() != 1)
        {
            throw new Exception("Must be 1 player");
        }

        _controls = world.GetPool<UnitControlComponent>();
        _weapons = world.GetPool<WeaponComponent>();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var playerId in _playerFilter)
        {
            ref WeaponComponent playerWeapon = ref _weapons.Get(playerId);
            ref var control = ref _controls.Get(playerId);

            playerWeapon.State = control.IsShoot ? WeaponState.WantsToFire : WeaponState.ReadyToFire;
        }
    }
}