using Leopotam.EcsLite;
using MysticEchoes.Core.Characters.Player;
using MysticEchoes.Core.Characters.Shooting;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Input.Player;

public class PlayerShootingSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] private InputManager _inputManager;
    
    private EcsFilter _playerFilter;
    private EcsPool<WeaponComponent> _weapons;
    private EcsPool<ShootRequest> _shootRequests;
    
    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _playerFilter = world.Filter<PlayerMarker>().Inc<WeaponComponent>().End();
        if (_playerFilter.GetEntitiesCount() != 1)
        {
            throw new Exception("Must be 1 player");
        }

        _weapons = world.GetPool<WeaponComponent>();
        _shootRequests = world.GetPool<ShootRequest>();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var playerId in _playerFilter)
        {
            if (!_inputManager.Shooting || _shootRequests.Has(playerId))
            {
                return;
            }
            
            _shootRequests.Add(playerId);
        }
    }
}