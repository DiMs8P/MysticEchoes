using Leopotam.EcsLite;
using MysticEchoes.Core.Config.Input;
using MysticEchoes.Core.Player;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Shooting;

public class PlayerShootingSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] private IInputManager _inputManager;
    
    private EcsFilter _playerFilter;
    private EcsPool<RangeWeaponComponent> _weapons;

    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _playerFilter = world.Filter<PlayerMarker>().Inc<RangeWeaponComponent>().End();
        if (_playerFilter.GetEntitiesCount() != 1)
        {
            throw new Exception("Must be 1 player");
        }

        _weapons = world.GetPool<RangeWeaponComponent>();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var playerId in _playerFilter)
        {
            ref RangeWeaponComponent playerRangeWeapon = ref _weapons.Get(playerId);
            playerRangeWeapon.IsShooting = _inputManager.IsShooting();
        }
    }
}