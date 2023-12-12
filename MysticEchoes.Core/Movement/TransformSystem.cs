using System.Numerics;
using Leopotam.EcsLite;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Movement;

public class TransformSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] private SystemExecutionContext _context;
    
    private EcsPool<TransformComponent> _transforms;
    private EcsPool<MovementComponent> _movements;
    private EcsFilter _transformsFilter;

    public void Init(IEcsSystems systems)
    {
        EcsWorld world = systems.GetWorld();
        
        _transforms = world.GetPool<TransformComponent>();
        _movements = world.GetPool<MovementComponent>();
        _transformsFilter = world.Filter<TransformComponent>().End();
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
    }
}