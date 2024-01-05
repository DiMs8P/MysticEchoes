using System.Numerics;
using Leopotam.EcsLite;
using MazeGeneration;
using MazeGeneration.TreeModule;
using MysticEchoes.Core.Animations;
using MysticEchoes.Core.Base.Geometry;
using MysticEchoes.Core.Collisions;
using MysticEchoes.Core.Collisions.Tree;
using MysticEchoes.Core.Configuration;
using MysticEchoes.Core.Items;
using MysticEchoes.Core.Loaders;
using MysticEchoes.Core.MapModule;
using MysticEchoes.Core.MapModule.Rooms;
using MysticEchoes.Core.Rendering;
using SevenBoldPencil.EasyDi;
using Point = System.Drawing.Point;

namespace MysticEchoes.Core.Scene;

public class InitEnvironmentSystem : IEcsInitSystem
{
    [EcsInject] private IMazeGenerator _mazeGenerator;
    [EcsInject] private Settings _settings;
    [EcsInject] private EntityBuilder _builder;
    [EcsInject] private ItemsFactory _itemsFactory;
    [EcsInject] private PrefabManager _prefabManager;

    private EcsPool<StaticCollider> _staticColliders;
    private EcsPool<DynamicCollider> _dynamicColliders;

    public void Init(IEcsSystems systems)
    {
        var world = systems.GetWorld();
        _staticColliders = world.GetPool<StaticCollider>();
        _dynamicColliders = world.GetPool<DynamicCollider>();

        CreateMap();
        CreateMoney();
    }

    private void CreateMap()
    {
        var map = _mazeGenerator.Generate();
        var mapComponent = new TileMapComponent(map);
        _builder.Create()
            .Add(mapComponent)
            .Add(new RenderComponent(RenderingType.TileMap))
            .End();

        CreateWalls(map, mapComponent);


        var doorEntities = CreateDoors(map);

        foreach (var roomNode in map.BinarySpaceTree.DeepCrawl()
                     .Where(x => x.Room is not null))
        {
            var room = roomNode.Room!.Shape;
            var doors = roomNode.Room.Doors;

            var roomBound = new Rectangle(
                new Vector2(room.X * mapComponent.TileSize.X, room.Y * mapComponent.TileSize.Y),
                new Vector2(mapComponent.TileSize.X, mapComponent.TileSize.Y)
            );
            var doorIds = doors.Select(x => doorEntities[x]).ToList();
            var enemySpawnIds = CreateEnemySpawn(roomNode, mapComponent);


            var roomId = _builder.Create()
                .Add(new RoomComponent
                {
                    Doors = doorIds,
                    Bound = roomBound,
                    EnemySpawns = enemySpawnIds
                })
                .End();

            var entranceTrigger = _builder.Create()
                .Add(new RenderComponent(RenderingType.DynamicColliderDebugView))
                .End();

            if (roomNode.Room.Type is not RoomType.PlayerSpawn)
            {
                _builder.AddTo(entranceTrigger, new DynamicCollider
                {
                    Box = new Box(
                        entranceTrigger,
                        new Rectangle(
                            new Vector2((room.Left + 1) * mapComponent.TileSize.X,
                                (room.Top + 1) * mapComponent.TileSize.Y),
                            new Vector2((room.Width - 1) * mapComponent.TileSize.X,
                                (room.Height - 1) * mapComponent.TileSize.Y)
                        )
                    ),
                    Behavior = CollisionBehavior.RoomEntranceTrigger
                });
                _builder.AddTo(entranceTrigger, new EntranceTrigger
                {
                    RoomId = roomId
                });
            }
        }
    }

    private Dictionary<Point, int> CreateDoors(Map map)
    {
        var doorEntities = new Dictionary<System.Drawing.Point, int>();
        foreach (var door in map.DoorTiles)
        {
            var doorId = _builder.Create()
                .Add(new DoorComponent
                {
                    IsOpen = true,
                    Tile = door
                })
                .End();
            doorEntities.Add(door, doorId);
        }

        return doorEntities;
    }

    private List<int> CreateEnemySpawn(RoomNode roomNode, TileMapComponent mapComponent)
    {
        var enemySpawnIds = new List<int>();
        foreach (var spawn in roomNode.Room.EnemySpawns)
        {
            var spawnId = _builder.Create()
                .End();
            enemySpawnIds.Add(spawnId);
            _builder.AddTo(spawnId, new DynamicCollider
                {
                    Box = new Box(
                        spawnId,
                        new Rectangle(
                            new Vector2(spawn.Area.Left * mapComponent.TileSize.X,
                                spawn.Area.Top * mapComponent.TileSize.Y),
                            new Vector2(spawn.Area.Width * mapComponent.TileSize.X,
                                spawn.Area.Height * mapComponent.TileSize.Y)
                        )
                    ),
                    Behavior = CollisionBehavior.Ignore
                })
                .AddTo(spawnId, new RenderComponent
                {
                    Type = RenderingType.EnemySpawn
                })
                .AddTo(spawnId, new EnemySpawnComponent
                {
                    Data = spawn
                });
        }

        return enemySpawnIds;
    }

    private void CreateWalls(Map map, TileMapComponent mapComponent)
    {
        foreach (var wall in map.WallTopTiles)
        {
            const float heightShift = 0.4f;
            const float colliderHeight = 1 - 2 * heightShift;
            var shape = new Rectangle(
                new Vector2(
                    wall.X * mapComponent.TileSize.X,
                    wall.Y * mapComponent.TileSize.Y + mapComponent.TileSize.Y * heightShift
                ),
                mapComponent.TileSize with { Y = mapComponent.TileSize.Y * colliderHeight }
            );
            CreateSingleWall(shape);
        }
        foreach (var wall in map.WallSideRightTiles)
        {
            var shape = new Rectangle(
                new Vector2(wall.X * mapComponent.TileSize.X, wall.Y * mapComponent.TileSize.Y),
                mapComponent.TileSize with
                {
                    X = mapComponent.TileSize.X * Map.ColliderThickness
                }
            );
            CreateSingleWall(shape);
        }
        foreach (var wall in map.WallSideLeftTiles)
        {
            var shape = new Rectangle(
                new Vector2(
                    wall.X * mapComponent.TileSize.X + mapComponent.TileSize.X * (1 - Map.ColliderThickness),
                    wall.Y * mapComponent.TileSize.Y
                ),
                mapComponent.TileSize with
                {
                    X = mapComponent.TileSize.X * Map.ColliderThickness
                }
            );
            CreateSingleWall(shape);
        }
        foreach (var wall in map.WallBottomTiles)
        {
            var shape = new Rectangle(
                new Vector2(
                    wall.X * mapComponent.TileSize.X,
                    wall.Y * mapComponent.TileSize.Y + mapComponent.TileSize.Y * Map.Shift
                ),
                mapComponent.TileSize with
                {
                    Y = mapComponent.TileSize.Y * Map.ColliderThickness
                }
            );
            CreateSingleWall(shape);
        }
        foreach (var wall in map.WallFullTiles)
        {
            var shape = new Rectangle(
                new Vector2(wall.X * mapComponent.TileSize.X, wall.Y * mapComponent.TileSize.Y),
                mapComponent.TileSize
            );
            CreateSingleWall(shape);
        }

        foreach (var wall in map.WallInnerCornerDownLeft)
        {
            var shape = new Rectangle(
                new Vector2(
                    wall.X * mapComponent.TileSize.X,
                    wall.Y * mapComponent.TileSize.Y + mapComponent.TileSize.Y * Map.Shift
                ),
                mapComponent.TileSize with
                {
                    Y = mapComponent.TileSize.Y * Map.ColliderThickness
                }
            );
            CreateSingleWall(shape);
            shape = new Rectangle(
                new Vector2(
                    wall.X * mapComponent.TileSize.X + mapComponent.TileSize.X * (1f - Map.ColliderThickness),
                    wall.Y * mapComponent.TileSize.Y
                ),
                new Vector2(
                    mapComponent.TileSize.X * Map.ColliderThickness,
                    mapComponent.TileSize.Y * Map.Shift
                )
            );
            CreateSingleWall(shape);

        }
        foreach (var wall in map.WallInnerCornerDownRight)
        {
            var shape = new Rectangle(
                new Vector2(
                    wall.X * mapComponent.TileSize.X,
                    wall.Y * mapComponent.TileSize.Y
                ),
                new Vector2(
                    mapComponent.TileSize.X * Map.ColliderThickness,
                    mapComponent.TileSize.Y * Map.Shift
                )
            );
            CreateSingleWall(shape);
            shape = new Rectangle(
                new Vector2(
                    wall.X * mapComponent.TileSize.X,
                    wall.Y * mapComponent.TileSize.Y + mapComponent.TileSize.Y * Map.Shift
                ),
                mapComponent.TileSize with
                {
                    Y = mapComponent.TileSize.Y * Map.ColliderThickness
                }
            );
            CreateSingleWall(shape);
        }

        foreach (var wall in map.WallDiagonalCornerDownLeft)
        {
            var shape = new Rectangle(
                new Vector2(
                    wall.X * mapComponent.TileSize.X + mapComponent.TileSize.X * 2 * Map.Shift,
                    wall.Y * mapComponent.TileSize.Y + mapComponent.TileSize.Y * (Map.Shift  + Map.ColliderThickness)
                ),
                new Vector2(
                    mapComponent.TileSize.X * Map.ColliderThickness,
                    mapComponent.TileSize.Y * (1 - (Map.Shift + Map.ColliderThickness))
                )
            );
            CreateSingleWall(shape);
        }
        foreach (var wall in map.WallDiagonalCornerDownRight)
        {
            var shape = new Rectangle(
                new Vector2(
                    wall.X * mapComponent.TileSize.X,
                    wall.Y * mapComponent.TileSize.Y + mapComponent.TileSize.Y * (Map.Shift + Map.ColliderThickness)
                ),
                new Vector2(
                    mapComponent.TileSize.X * Map.ColliderThickness,
                    mapComponent.TileSize.Y * (1 - (Map.Shift + Map.ColliderThickness))
                )
            );
            CreateSingleWall(shape);
        }
        foreach (var wall in map.WallDiagonalCornerUpLeft)
        {
            var shape = new Rectangle(
                new Vector2(
                    wall.X * mapComponent.TileSize.X + mapComponent.TileSize.X * 2 * Map.Shift,
                    wall.Y * mapComponent.TileSize.Y
                ),
                new Vector2(
                    mapComponent.TileSize.X * Map.ColliderThickness,
                    mapComponent.TileSize.Y * (1 - (Map.Shift + Map.ColliderThickness))
                )
            );
            CreateSingleWall(shape);
        }
        foreach (var wall in map.WallDiagonalCornerUpRight)
        {
            var shape = new Rectangle(
                new Vector2(
                    wall.X * mapComponent.TileSize.X,
                    wall.Y * mapComponent.TileSize.Y
                ),
                new Vector2(
                    mapComponent.TileSize.X * Map.ColliderThickness,
                    mapComponent.TileSize.Y * (1 - (Map.Shift + Map.ColliderThickness))
                )
            );
            CreateSingleWall(shape);
        }

    }

    private int CreateSingleWall(Rectangle shape)
    {
        var wallEntityId = _builder.Create()
            .Add(new StaticCollider
            {
                Box = new Box(
                    0,
                    shape
                ),
                Behavior = CollisionBehavior.Wall
            })
            .Add(new RenderComponent(RenderingType.StaticColliderDebugView))
            .End();
        ref var collider = ref _staticColliders.Get(wallEntityId);
        collider.Box.Id = wallEntityId;

        return wallEntityId;
    }

    private void CreateMoney()
    {
        int entityId = _itemsFactory.CreateItemEntity(0, 100);

        ref DynamicCollider collider = ref _dynamicColliders.Get(entityId);
        collider.Box = new Box(entityId, new Rectangle(
            Vector2.Zero,
            Vector2.One * 0.4f * 0.1f / 2
        ));
        collider.Behavior = CollisionBehavior.Item;
    }
}