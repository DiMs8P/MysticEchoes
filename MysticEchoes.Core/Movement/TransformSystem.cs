using System.Numerics;
using System.Runtime.Intrinsics;
using Leopotam.EcsLite;
using MysticEchoes.Core.Control;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Movement;

public class TransformSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] private SystemExecutionContext _context;

    private EcsFilter _controlsFilter;

    private EcsPool<TransformComponent> _transforms;
    private EcsPool<MovementComponent> _movements;
    private EcsPool<UnitControlComponent> _controls;
    private EcsFilter _transformsFilter;

    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();

        _controlsFilter = world.Filter<UnitControlComponent>().End();

        _transforms = world.GetPool<TransformComponent>();
        _movements = world.GetPool<MovementComponent>();
        _controls = world.GetPool<UnitControlComponent>();
        _transformsFilter = world.Filter<TransformComponent>().Inc<MovementComponent>().End();
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
    }
}