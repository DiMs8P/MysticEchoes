using Leopotam.EcsLite;
using MysticEchoes.Core.Input;
using MysticEchoes.Core.Player;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Shooting;

public class PlayerShootingSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] private IInputManager _inputManager;
    
    private EcsFilter _playerFilter;
    private EcsPool<WeaponOwnerComponent> _weapons;

    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _playerFilter = world.Filter<PlayerMarker>().Inc<WeaponOwnerComponent>().End();
        if (_playerFilter.GetEntitiesCount() != 1)
        {
            throw new Exception("Must be 1 player");
        }

        _weapons = world.GetPool<WeaponOwnerComponent>();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var playerId in _playerFilter)
        {
            ref WeaponOwnerComponent playerWeaponOwner = ref _weapons.Get(playerId);
            playerWeaponOwner.IsShooting = _inputManager.IsShooting();
        }
    }
}