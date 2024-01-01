using Leopotam.EcsLite;
using MysticEchoes.Core.Movement;
using SevenBoldPencil.EasyDi;
using System.Numerics;

namespace MysticEchoes.Core.Collisions;

public class CollidersMovementSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] private SystemExecutionContext _context;
    private EcsFilter _entityWithCollider;
    private EcsPool<MovementComponent> _movements;
    private EcsPool<DynamicCollider> _colliders;
    private EcsPool<TransformComponent> _transform;

    public void Init(IEcsSystems systems)
    {
        var world = systems.GetWorld();

        _transform = world.GetPool<TransformComponent>();
        _movements = world.GetPool<MovementComponent>();
        _colliders = world.GetPool<DynamicCollider>();

        _entityWithCollider = world.Filter<DynamicCollider>()
            .Inc<MovementComponent>()
            .Inc<TransformComponent>()
            .End();

    }

    public void Run(IEcsSystems systems)
    {
        foreach (var entity in _entityWithCollider)
        {
            ref var movement = ref _movements.Get(entity);
            ref var collider = ref _colliders.Get(entity);
            ref var transform = ref _transform.Get(entity);

            collider.Box.Shape = collider.Box.Shape with
            {
                LeftBottom = transform.Location - collider.Box.Shape.Size / 2
            };

        }
    }
}