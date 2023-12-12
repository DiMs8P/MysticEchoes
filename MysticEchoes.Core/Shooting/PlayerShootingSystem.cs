using Leopotam.EcsLite;
using MysticEchoes.Core.Input;
using MysticEchoes.Core.Player;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Shooting;

public class PlayerShootingSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] private IInputManager _inputManager;
    
    private EcsFilter _playerFilter;
    private EcsPool<ShootRequest> _shootRequests;
    
    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _playerFilter = world.Filter<PlayerMarker>().Inc<WeaponComponent>().End();
        if (_playerFilter.GetEntitiesCount() != 1)
        {
            throw new Exception("Must be 1 player");
        }

        _shootRequests = world.GetPool<ShootRequest>();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var playerId in _playerFilter)
        {
            if (!_inputManager.IsShooting() || _shootRequests.Has(playerId))
            {
                return;
            }
            
            _shootRequests.Add(playerId);
        }
    }
}