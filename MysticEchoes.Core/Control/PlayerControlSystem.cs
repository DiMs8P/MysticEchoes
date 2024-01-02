using System.Numerics;
using Leopotam.EcsLite;
using MysticEchoes.Core.Config.Input;
using MysticEchoes.Core.Movement;
using MysticEchoes.Core.Player;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Control;

public class PlayerControlSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] private IInputManager _inputManager;

    private EcsFilter _playerFilter;
    private EcsPool<UnitControlComponent> _controls;

    public void Init(IEcsSystems systems)
    {
        var world = systems.GetWorld();

        _playerFilter = world.Filter<PlayerMarker>().Inc<TransformComponent>().End();
        if (_playerFilter.GetEntitiesCount() != 1)
        {
            throw new Exception("Must be 1 player");
        }

        _controls = world.GetPool<UnitControlComponent>();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var playerId in _playerFilter)
        {
            ref var playerControl = ref _controls.Get(playerId);
            playerControl.IsShoot = _inputManager.IsShooting();
            playerControl.MoveDirection = 
                Vector2.UnitX * _inputManager.GetHorizontal() +
                Vector2.UnitY * _inputManager.GetVertical();
            playerControl.LookAt = playerControl.MoveDirection;
        }
    }
}