using Leopotam.EcsLite;
using MysticEchoes.Core.Input;
using MysticEchoes.Core.Player;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Shooting;

public class PlayerShootingSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] private IInputManager _inputManager;
    
    private EcsFilter _playerFilter;
    private EcsPool<OwnerComponent> _weapons;

    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _playerFilter = world.Filter<PlayerMarker>().Inc<OwnerComponent>().End();
        if (_playerFilter.GetEntitiesCount() != 1)
        {
            throw new Exception("Must be 1 player");
        }

        _weapons = world.GetPool<OwnerComponent>();
    }

    public void Run(IEcsSystems systems)
    {
        /*foreach (var playerId in _playerFilter)
        {
            ref WeaponComponent playerWeapon = ref _weapons.Get(playerId);
            playerWeapon.State = _inputManager.IsShooting() ? WeaponState.WantsToFire : WeaponState.ReadyToFire;
        }*/
    }
}