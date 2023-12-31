using System.Numerics;
using System.Reflection;
using Leopotam.EcsLite;
using MysticEchoes.Core.Base.Geometry;
using MysticEchoes.Core.Collisions.Tree;
using MysticEchoes.Core.MapModule;
using MysticEchoes.Core.Movement;
using MysticEchoes.Core.Rendering;
using MysticEchoes.Core.Scene;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Collisions;

public class CollisionsSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] private EntityFactory _factory;
    [EcsInject] private SystemExecutionContext _context;

    private int _mapId;
    private EcsFilter _staticEntities;
    private EcsFilter _dynamicEntities;
    private EcsPool<StaticCollider> _staticColliders;
    private EcsPool<DynamicCollider> _dynamicColliders;
    private QuadTree _staticCollidersTree;
    private EcsFilter _dynamicCollidersFilter;
    private QuadTree _dynamicCollidersTree;
    private EcsPool<TransformComponent> _transforms;
    private EcsPool<MovementComponent> _movements;
    private const float CollisionResolvingSensitivity = 1e-4f;

    public void Init(IEcsSystems systems)
    {
        var world = systems.GetWorld();

        var mapFilter = world.Filter<TileMapComponent>().End();
        if (mapFilter.GetEntitiesCount() != 1)
        {
            throw new Exception("Must be 1 map");
        }
        _mapId = mapFilter.GetRawEntities()[0];

        _staticEntities = world.Filter<StaticCollider>().End();
        _dynamicEntities = world.Filter<DynamicCollider>().End();
        _staticColliders = world.GetPool<StaticCollider>();
        _dynamicColliders = world.GetPool<DynamicCollider>();
        _transforms = world.GetPool<TransformComponent>();
        _movements = world.GetPool<MovementComponent>();

        _dynamicCollidersFilter = world.Filter<DynamicCollider>()
            .Inc<MovementComponent>()
            .End();

        ref var map = ref world.GetPool<TileMapComponent>().Get(_mapId);

        _staticCollidersTree = new QuadTree(
            new Rectangle(
                new Vector2(0, 0),
                new Vector2(map.Tiles.Size.Width * map.TileSize.X, map.Tiles.Size.Height * map.TileSize.Y)
            ),
            4
        );
        _dynamicCollidersTree = new QuadTree(
            new Rectangle(
                new Vector2(0, 0),
                new Vector2(map.Tiles.Size.Width * map.TileSize.X, map.Tiles.Size.Height * map.TileSize.Y)
            ),
            4
        );
        foreach (var staticEntity in _staticEntities)
        {
            var collider = _staticColliders.Get(staticEntity);
            _staticCollidersTree.Add(collider.Box);
        }

        //_factory.Create()
        //    .Add(new SpaceTreeComponent(){Tree = _staticCollidersTree})
        //    .Add(new RenderComponent(RenderingType.ColliderSpaceTreeView));
        _factory.Create()
            .Add(new SpaceTreeComponent() { Tree = _dynamicCollidersTree })
            .Add(new RenderComponent(RenderingType.ColliderSpaceTreeView));
    }

    public void Run(IEcsSystems systems)
    {
        _dynamicCollidersTree.Clear();

        foreach (var entity in _dynamicCollidersFilter)
        {
            var collider = _dynamicColliders.Get(entity);

            _dynamicCollidersTree.Add(collider.Box);
        }

        foreach (var entity in _dynamicCollidersFilter)
        {
            ref var transform = ref _transforms.Get(entity);
            var collider = _dynamicColliders.Get(entity);
            var box = collider.Box;

            var intersectedDynamicEntities = _dynamicCollidersTree.Query(box.Shape);
            intersectedDynamicEntities.Remove(entity);

            var staticIntersected = _staticCollidersTree.Query(box.Shape);

            foreach (var target in intersectedDynamicEntities)
            {
                var targetCollider = _dynamicColliders.Get(target);

                //HandleCollisions(new CollisionHandlingEntityInfo
                //{
                //    Id = entity,
                //    Behavior = collider.Behavior,
                //    Box = box
                //}, new CollisionHandlingEntityInfo
                //{
                //    Id = target,
                //    Behavior = targetCollider.Behavior,
                //    Box = targetCollider.Box
                //});
            }

            foreach (var target in staticIntersected)
            {
                var targetCollider = _staticColliders.Get(target);
                
                //HandleCollisions(new CollisionHandlingEntityInfo
                //{
                //    Id = entity,
                //    Behavior = collider.Behavior,
                //    Box = box
                //}, new CollisionHandlingEntityInfo
                //{
                //    Id = target,
                //    Behavior = targetCollider.Behavior,
                //    Box = targetCollider.Box
                //});
            }
        }
    }

    private void HandleCollisions(CollisionHandlingEntityInfo entity, CollisionHandlingEntityInfo target)
    {
        if (entity.Behavior is CollisionBehavior.Wall)
        {
            return;
        }
        if (entity.Behavior is CollisionBehavior.None || target.Behavior is CollisionBehavior.None)
        {
            throw new InvalidOperationException();
        }

        if (target.Behavior is CollisionBehavior.Wall &&
            entity.Behavior is CollisionBehavior.AllyCharacter or CollisionBehavior.EnemyCharacter
            )
        {
            var shape = entity.Box.Shape;
            var targetShape = target.Box.Shape;
            ref var transform = ref _transforms.Get(entity.Id);
            ref var movement = ref _movements.Get(entity.Id);
            if (movement.Velocity == Vector2.Zero)
            {
                return;
            }

            //var k = 0;
            if (movement.Velocity.Y > 0 && (targetShape.Bottom <= shape.Top && shape.Top <= targetShape.Top))
            {
                transform.Location = transform.Location with { Y = targetShape.Bottom - shape.Size.Y / 2 - 1e-5f };
            }
            else if (movement.Velocity.Y < 0 && (targetShape.Bottom <= shape.Bottom && shape.Bottom <= targetShape.Top))
            {
                transform.Location = transform.Location with { Y = targetShape.Top + shape.Size.Y / 2 + 1e-3f };
            }
            
            if (movement.Velocity.X > 0 && (targetShape.Left <= shape.Right && shape.Right <= targetShape.Right))
            {
                transform.Location = transform.Location with { X = targetShape.Left - shape.Size.X / 2 - 1e-5f };
            }
            else if (movement.Velocity.X < 0 && (targetShape.Left <= shape.Left && shape.Left <= targetShape.Right))
            {
                transform.Location = transform.Location with { X = targetShape.Right + shape.Size.X / 2 + 1e-5f };
            }
            //while (targetShape.Intersects(shape))
            //{
            //    k++;
            //    transform.Location -= movement.Velocity * CollisionResolvingSensitivity;
            //    shape = shape with { LeftBottom = shape.LeftBottom - movement.Velocity * CollisionResolvingSensitivity };
            //}

            //if (targetShape.Left <= shape.Right && shape.Right <= targetShape.Right)
            //{
            //    transform.Location = transform.Location with { X = targetShape.Left - shape.Size.X / 2 - 1e-13f };
            //    return;
            //}
            //if (targetShape.Left <= shape.Left && shape.Left <= targetShape.Right)
            //{
            //    transform.Location = transform.Location with { X = targetShape.Right + shape.Size.X / 2 + 1e-13f };
            //    return;
            //}
            //if (targetShape.Bottom <= shape.Top && shape.Top <= targetShape.Top)
            //{
            //    transform.Location = transform.Location with { Y = targetShape.Bottom - shape.Size.Y / 2 - 1e-13f };
            //    return;
            //}
            //if (targetShape.Bottom <= shape.Bottom && shape.Bottom <= targetShape.Top)
            //{
            //    transform.Location = transform.Location with { Y = targetShape.Top + shape.Size.Y / 2 + 1e-13f };
            //    return;
            //}
            // Подвинуть героя 
            return;
        }
        if (entity.Behavior is CollisionBehavior.AllyBullet or CollisionBehavior.EnemyBullet)
        {
            if (target.Behavior is CollisionBehavior.Wall)
            {
                // Уничтожить снаряд
            }
            return;
        }

        if (entity.Behavior is CollisionBehavior.AllyCharacter)
        {
            if (target.Behavior is CollisionBehavior.EnemyBullet)
            {
                // Нанести урон
                return;
            }

            return;
        }
        if (entity.Behavior is CollisionBehavior.EnemyCharacter)
        {
            if (target.Behavior is CollisionBehavior.AllyBullet)
            {
                // Нанести урон
                return;
            }

            return;
        }

        throw new NotImplementedException($"collision {entity.Behavior} and {target.Behavior} not implemented");
    }
}

readonly ref struct CollisionHandlingEntityInfo
{
    public int Id { get; init; }
    public Box Box { get; init; }
    public CollisionBehavior Behavior { get; init; }
}