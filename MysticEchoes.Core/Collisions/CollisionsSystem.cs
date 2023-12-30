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

    private int _mapId;
    private EcsFilter _staticEntities;
    private EcsFilter _dynamicEntities;
    private EcsPool<StaticCollider> _staticColliders;
    private EcsPool<DynamicCollider> _dynamicColliders;
    private QuadTree _staticCollidersTree;
    private EcsFilter _dynamicCollidersFilter;
    private QuadTree _dynamicCollidersTree;
    private EcsPool<TransformComponent> _transforms;

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
            ref var transform = ref _transforms.Get(entity);
            var collider = _dynamicColliders.Get(entity);

            var transformedBox = GetTransformedBox(collider, transform);

            _dynamicCollidersTree.Add(transformedBox);
        }

        foreach (var entity in _dynamicCollidersFilter)
        {
            ref var transform = ref _transforms.Get(entity);
            var collider = _dynamicColliders.Get(entity);
            var transformedBox = GetTransformedBox(collider, transform);

            var intersectedDynamicEntities = _dynamicCollidersTree.Query(transformedBox.Shape);
            intersectedDynamicEntities.Remove(entity);

            var staticIntersected = _staticCollidersTree.Query(transformedBox.Shape);

            foreach (var target in intersectedDynamicEntities)
            {
                ref var targetTransform = ref _transforms.Get(target);
                var targetCollider = _dynamicColliders.Get(target);
                var transformedTargetBox = GetTransformedBox(targetCollider, targetTransform);

                HandleCollisions(new CollisionHandlingEntityInfo
                {
                    Id = entity,
                    Behavior = collider.Behavior,
                    Box = transformedBox
                }, new CollisionHandlingEntityInfo
                {
                    Id = target,
                    Behavior = targetCollider.Behavior,
                    Box = transformedTargetBox
                });
            }

            if (staticIntersected.Count > 0)
            {
                Console.WriteLine(123);
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
            // Подвинуть героя 
            return;
        }
        if (target.Behavior is CollisionBehavior.Wall &&
            entity.Behavior is CollisionBehavior.AllyBullet or CollisionBehavior.EnemyBullet
           )
        {
            // Уничтожить снаряд
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

    private static Box GetTransformedBox(DynamicCollider collider, TransformComponent transform)
    {
        var transformedCollider = collider.Box with
        {
            Shape = collider.Box.Shape with
            {
                LeftBottom = collider.Box.Shape.LeftBottom + transform.Location
            }
        };
        return transformedCollider;
    }
}

readonly ref struct CollisionHandlingEntityInfo
{
    public int Id { get; init; }
    public Box Box { get; init; }
    public CollisionBehavior Behavior { get; init; }
}