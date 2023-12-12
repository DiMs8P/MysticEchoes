using System.Numerics;
using Leopotam.EcsLite;
using MysticEchoes.Core.Input;
using MysticEchoes.Core.Player;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Movement;

public class PlayerMovementSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] private IInputManager _inputManager;
    
    private EcsFilter _playerFilter;
    private EcsPool<MovementComponent> _movements;
    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _playerFilter = world.Filter<PlayerMarker>().Inc<TransformComponent>().End();
        if (_playerFilter.GetEntitiesCount() != 1)
        {
            throw new Exception("Must be 1 player");
        }
        
        _movements = world.GetPool<MovementComponent>();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var playerId in _playerFilter)
        {
            ref MovementComponent movementComponent = ref _movements.Get(playerId);
            movementComponent.Velocity = Vector2.Zero;
            
            if (Math.Abs(_inputManager.GetVertical()) < 0.001 && Math.Abs( _inputManager.GetHorizontal()) < 0.001)
            {
                return;
            }
            
            movementComponent.Velocity += _inputManager.GetVertical() * Vector2.UnitY;
            movementComponent.Velocity += _inputManager.GetHorizontal() * Vector2.UnitX;
        }
    }
}