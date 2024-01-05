using System.Numerics;
using Leopotam.EcsLite;
using MysticEchoes.Core.Base.Geometry;
using MysticEchoes.Core.Collisions.Tree;
using MysticEchoes.Core.Items;
using MysticEchoes.Core.Loaders;
using MysticEchoes.Core.Loaders.Prefabs;
using MysticEchoes.Core.MapModule;
using MysticEchoes.Core.MapModule.Rooms;
using MysticEchoes.Core.Movement;
using MysticEchoes.Core.Rendering;
using MysticEchoes.Core.Scene;
using MysticEchoes.Core.Shooting;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Collisions;

public class CollisionsSystem : IEcsInitSystem, IEcsRunSystem
{
    [EcsInject] private EntityBuilder _builder;
    [EcsInject] private PrefabManager _prefabManager;
    [EcsInject] private SystemExecutionContext _context;

    private EcsWorld _world;

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
    private EcsPool<ItemComponent> _items;
    private EcsPool<ExplosionComponent> _explosions;
    private EcsPool<EntranceTrigger> _entranceTriggers;
    private EcsPool<RoomComponent> _rooms;
    private EcsPool<DoorComponent> _doors;
    private const float CollisionResolvingSensitivity = 1e-4f;
    private List<int> _entitiesToClear = new List<int>();

    private HashSet<int> _deletedEntities = new HashSet<int>();

    public void Init(IEcsSystems systems)
    {
        _world = systems.GetWorld();

        var mapFilter = _world.Filter<TileMapComponent>().End();
        if (mapFilter.GetEntitiesCount() != 1)
        {
            throw new Exception("Must be 1 map");
        }
        _mapId = mapFilter.GetRawEntities()[0];

        _staticEntities = _world.Filter<StaticCollider>().End();
        _dynamicEntities = _world.Filter<DynamicCollider>().End();
        _staticColliders = _world.GetPool<StaticCollider>();
        _dynamicColliders = _world.GetPool<DynamicCollider>();
        _explosions = _world.GetPool<ExplosionComponent>();
        _transforms = _world.GetPool<TransformComponent>();
        _movements = _world.GetPool<MovementComponent>();
        _items = _world.GetPool<ItemComponent>();
        _entranceTriggers = _world.GetPool<EntranceTrigger>();
        _rooms = _world.GetPool<RoomComponent>();
        _doors = _world.GetPool<DoorComponent>();

        _dynamicCollidersFilter = _world.Filter<DynamicCollider>()
            .End();
        
        ref var map = ref _world.GetPool<TileMapComponent>().Get(_mapId);

        _staticCollidersTree = new QuadTree(
            new Rectangle(
                new Vector2(0, 0),
                new Vector2(map.Map.Size.Width * map.TileSize.X, map.Map.Size.Height * map.TileSize.Y)
            ),
            10
        );
        _dynamicCollidersTree = new QuadTree(
            new Rectangle(
                new Vector2(0, 0),
                new Vector2(map.Map.Size.Width * map.TileSize.X, map.Map.Size.Height * map.TileSize.Y)
            ),
            20
        );
        foreach (var staticEntity in _staticEntities)
        {
            var collider = _staticColliders.Get(staticEntity);
            _staticCollidersTree.Add(collider.Box);
        }

        //_builder.Create()
        //    .Add(new SpaceTreeComponent() { Tree = _staticCollidersTree })
        //    .Add(new RenderComponent(RenderingType.ColliderSpaceTreeView));
        //_builder.Create()
        //    .Add(new SpaceTreeComponent() { Tree = _dynamicCollidersTree })
        //    .Add(new RenderComponent(RenderingType.ColliderSpaceTreeView));
    }

    public void Run(IEcsSystems systems)
    {
        _entitiesToClear.Clear();
        _dynamicCollidersTree.Clear();
        _deletedEntities.Clear();

        foreach (var entity in _dynamicCollidersFilter)
        {
            var collider = _dynamicColliders.Get(entity);
            _dynamicCollidersTree.Add(collider.Box);
        }

        foreach (var entity in _dynamicCollidersFilter)
        {
            var collider = _dynamicColliders.Get(entity);
            var box = collider.Box;

            var intersectedDynamicEntities = _dynamicCollidersTree.Query(box.Shape);
            intersectedDynamicEntities.Remove(entity);

            var staticIntersected = _staticCollidersTree.Query(box.Shape);

            foreach (var target in intersectedDynamicEntities)
            {
                var targetCollider = _dynamicColliders.Get(target);

                HandleCollisions(new CollisionHandlingEntityInfo
                {
                    Id = entity,
                    Behavior = collider.Behavior,
                    Box = box
                }, new CollisionHandlingEntityInfo
                {
                    Id = target,
                    Behavior = targetCollider.Behavior,
                    Box = targetCollider.Box
                });
            }

            foreach (var target in staticIntersected)
            {
                var targetCollider = _staticColliders.Get(target);

                HandleCollisions(new CollisionHandlingEntityInfo
                {
                    Id = entity,
                    Behavior = collider.Behavior,
                    Box = box
                }, new CollisionHandlingEntityInfo
                {
                    Id = target,
                    Behavior = targetCollider.Behavior,
                    Box = targetCollider.Box
                });
            }
        }

        foreach (var entity in _entitiesToClear)
        {
            _world.DelEntity(entity);
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
            HandleCharacterAndWallCollision(entity, target);

            return;
        }
        if (entity.Behavior is CollisionBehavior.AllyBullet or CollisionBehavior.EnemyBullet)
        {
            if (target.Behavior is CollisionBehavior.Wall)
            {
                if (!_deletedEntities.Contains(entity.Id))
                {
                    if (_explosions.Has(entity.Id))
                    {
                        ref ExplosionComponent explosionComponent = ref _explosions.Get(entity.Id);
                    
                        int explosionId = _prefabManager.CreateEntityFromPrefab(_builder, explosionComponent.ExplosionPrefab);

                        ref TransformComponent explosionTransform =  ref _transforms.Get(explosionId);
                        ref TransformComponent bulletTransform =  ref _transforms.Get(entity.Id);

                        explosionTransform.Location = bulletTransform.Location;
                        
                        ref var explosionCollider = ref _dynamicColliders.Get(explosionId);
                        Vector2 boxSize = new Vector2(0.265f, 0.35f) * explosionTransform.Scale;
                        explosionCollider.Box = new Box(explosionId, new Rectangle(
                            explosionTransform.Location - boxSize / 2, 
                            boxSize
                        ));
                        explosionCollider.Behavior = CollisionBehavior.Ignore;
                    }
                
                    _entitiesToClear.Add(entity.Id);
                }
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

            if (target.Behavior is CollisionBehavior.Item)
            {
                if (!_deletedEntities.Contains(target.Id))
                {
                    ref ItemComponent itemComponent = ref _items.Get(target.Id);
                    itemComponent.Item.OnItemTaken(entity.Id, _world);
                    
                    DeleteEntity(target.Id);
                }
                
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
        
        if (entity.Behavior is CollisionBehavior.Item)
        {
            return;
        }

        if (entity.Behavior is CollisionBehavior.RoomEntranceTrigger)
        {
            if (target.Behavior == CollisionBehavior.AllyCharacter)
            {
                ref var trigger = ref _entranceTriggers.Get(entity.Id);
                if (trigger.IsActivated)
                {
                    return;
                }
                trigger.IsActivated = true;
                _entitiesToClear.Add(entity.Id);

                var room = _rooms.Get(trigger.RoomId);

                foreach (var doorId in room.Doors)
                {
                    ref var door = ref _doors.Get(doorId);

                    door.IsOpen = false;
                    _builder.AddTo(doorId, new DynamicCollider
                    {
                        Box = new Box(
                            doorId,
                            door.Shape
                        ),
                        Behavior = CollisionBehavior.Wall
                    });
                }
            }
            return;
        }
        if (entity.Behavior is CollisionBehavior.Ignore)
        {
            return;
        }
        throw new NotImplementedException($"collision {entity.Behavior} and {target.Behavior} not implemented");
    }

    private void HandleCharacterAndWallCollision(CollisionHandlingEntityInfo entity, CollisionHandlingEntityInfo target)
    {
        ref var transform = ref _transforms.Get(entity.Id);
        ref var movement = ref _movements.Get(entity.Id);
        if (movement.Velocity == Vector2.Zero)
        {
            return;
        }

        var P1 = transform.Location;
        var P0 = transform.Location - movement.Speed * movement.Velocity * _context.DeltaTime;
        var shift = P1 - P0;
        var h = entity.Box.Shape.Size / 2;
        var expandedTarget = new Rectangle
        {
            LeftBottom = target.Box.Shape.LeftBottom - h,
            Size = target.Box.Shape.Size + 2 * h
        };
        var tNear = (expandedTarget.LeftBottom - P0) / shift;
        var tFar = (expandedTarget.RightTop - P0) / shift;
        if (tNear.X > tFar.X)
        {
            (tNear.X, tFar.X) = (tFar.X, tNear.X);
        }

        if (tNear.Y > tFar.Y)
        {
            (tNear.Y, tFar.Y) = (tFar.Y, tNear.Y);
        }

        var tHitNear = float.Max(tNear.X, tNear.Y);
        var tHitFar = float.Min(tFar.X, tFar.Y);
        if (tHitFar < 0)
        {
            return;
        }

        var contactNormal = GetContactNormal(tNear, shift);

        var rollback = contactNormal * Vector2.Abs(shift) * (1f - tHitNear + CollisionResolvingSensitivity);
        transform.Location += rollback;
        ref var collider = ref _dynamicColliders.Get(entity.Id);
        collider.Box.Shape = collider.Box.Shape with
        {
            LeftBottom = collider.Box.Shape.LeftBottom + rollback
        };
        return;
    }

    private Vector2 GetContactNormal(Vector2 tNear, Vector2 rayDirection)
    {
        if (tNear.X > tNear.Y)
        {
            if (rayDirection.X < 0)
            {
                return Vector2.UnitX;
            }
            return -1f * Vector2.UnitX;
        }
        if (tNear.X < tNear.Y)
        {
            if (rayDirection.Y < 0)
            {
                return Vector2.UnitY;
            }
            return -1f * Vector2.UnitY;
        }
        return Vector2.Zero;
    }
    
    private void DeleteEntity(int entityId)
    {
        var collider = _dynamicColliders.Get(entityId);
        var box = collider.Box;
        _dynamicCollidersTree.Remove(box);
        
        _world.DelEntity(entityId);
        _deletedEntities.Add(entityId);
    }
}

readonly ref struct CollisionHandlingEntityInfo
{
    public int Id { get; init; }
    public Box Box { get; init; }
    public CollisionBehavior Behavior { get; init; }
}