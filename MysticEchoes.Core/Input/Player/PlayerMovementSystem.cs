using System.Numerics;
using Leopotam.EcsLite;
using MysticEchoes.Core.Character.Player;
using MysticEchoes.Core.Movement;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Input.Player;

public class PlayerMovementSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] private InputManager _inputManager;
    
    private EcsFilter _playerFilter;
    private EcsPool<TransformComponent> _transforms;
    private EcsPool<MovementComponent> _movements;
    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _playerFilter = world.Filter<PlayerMarker>().Inc<TransformComponent>().End();
        if (_playerFilter.GetEntitiesCount() != 1)
        {
            throw new Exception("Must be 1 player");
        }
        
        _transforms = world.GetPool<TransformComponent>();
        _movements = world.GetPool<MovementComponent>();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var playerId in _playerFilter)
        {
            ref MovementComponent movementComponent = ref _movements.Get(playerId);
            movementComponent.Velocity = Vector2.Zero;
            
            if (Math.Abs( _inputManager.Vertical) < 0.001 && Math.Abs( _inputManager.Horizontal) < 0.001)
            {
                return;
            }
            
            movementComponent.Velocity = Vector2.Zero;
            movementComponent.Velocity += _inputManager.Vertical * Vector2.UnitY;
            movementComponent.Velocity += _inputManager.Horizontal * Vector2.UnitX;

        }
    }
}