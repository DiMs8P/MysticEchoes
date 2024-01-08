using System.Numerics;
using System.Runtime.Intrinsics;
using Leopotam.EcsLite;
using MysticEchoes.Core.Camera;
using MysticEchoes.Core.Control;
using MysticEchoes.Core.Player;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Movement;

public class TransformSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] private SystemExecutionContext _context;

    private EcsFilter _controlsFilter;
    private EcsFilter _playerFilter;

    private EcsPool<TransformComponent> _transforms;
    private EcsPool<MovementComponent> _movements;
    private EcsPool<UnitControlComponent> _controls;
    private EcsFilter _transformsFilter;
    private EcsPool<CameraComponent> _camera;

    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _controlsFilter = world.Filter<UnitControlComponent>().End();
        _playerFilter = world.Filter<PlayerMarker>().Inc<TransformComponent>().End();
        if (_playerFilter.GetEntitiesCount() != 1)
        {
            throw new Exception("Must be 1 player");
        }

        _transforms = world.GetPool<TransformComponent>();
        _movements = world.GetPool<MovementComponent>();
        _controls = world.GetPool<UnitControlComponent>();
        _transformsFilter = world.Filter<TransformComponent>().Inc<MovementComponent>().End();
        _camera = world.GetPool<CameraComponent>();
    }

    public void Run(IEcsSystems systems)
    {
        foreach (var entityId in _transformsFilter)
        {
            ref TransformComponent transform = ref _transforms.Get(entityId);
            ref MovementComponent movement = ref _movements.Get(entityId);


            if (movement.Velocity.IsNearlyZero())
            {
                continue;
            }

            movement.Velocity = Vector2.Normalize(movement.Velocity);

            transform.Location += movement.Speed * movement.Velocity * _context.DeltaTime;
            transform.Rotation = movement.Velocity;
        }

        foreach (var entityId in _controlsFilter)
        {
            ref TransformComponent transform = ref _transforms.Get(entityId);
            ref var control = ref _controls.Get(entityId);
            transform.Rotation = Vector2.Normalize(control.LookAt - transform.Location);
        }
        foreach(var playerId in _playerFilter)
        {
            ref var camera = ref _camera.Get(playerId);
            ref TransformComponent transform = ref _transforms.Get(playerId);
            ref var control = ref _controls.Get(playerId);
            Vector2 loc = new(transform.Location.X - camera.Position.X, transform.Location.Y - camera.Position.Y);
            transform.Rotation = Vector2.Normalize(control.LookAt - loc);
        }
    }
}