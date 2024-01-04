using System.Numerics;
using Leopotam.EcsLite;
using MysticEchoes.Core.Base.Geometry;
using MysticEchoes.Core.Collisions;
using MysticEchoes.Core.Collisions.Tree;
using MysticEchoes.Core.Configuration;
using MysticEchoes.Core.Items;
using MysticEchoes.Core.Loaders;
using MysticEchoes.Core.MapModule;
using MysticEchoes.Core.Movement;
using MysticEchoes.Core.Rendering;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Scene;

public class InitEnvironmentSystem : IEcsInitSystem
{
    [EcsInject] private IMazeGenerator _mazeGenerator;
    [EcsInject] private Settings _settings;
    [EcsInject] private EntityBuilder _builder;
    [EcsInject] private ItemsFactory _itemsFactory;
    private EcsPool<StaticCollider> _staticColliders;
    private EcsPool<DynamicCollider> _dynamicColliders;

    [EcsInject] private PrefabManager _prefabManager;
    public void Init(IEcsSystems systems)
    {
        var world = systems.GetWorld();
        _staticColliders = world.GetPool<StaticCollider>();
        _dynamicColliders = world.GetPool<DynamicCollider>();

        CreateTiles();

        //CreateSquare();
        CreateItem();
    }

    private void CreateTiles()
    {
        var map = _mazeGenerator.Generate();

        var mapComponent = new TileMapComponent(map);
        _builder.Create()
            .Add(mapComponent)
            .Add(new RenderComponent(RenderingType.TileMap))
            .End();

        foreach (var wall in map.WallTiles)
        {
            var wallEntityId = _builder.Create()
                .Add(new StaticCollider
                {
                    Box = new Box(
                        0,
                        new Rectangle(
                            new Vector2(wall.X * mapComponent.TileSize.X, wall.Y * mapComponent.TileSize.Y),
                            new Vector2(mapComponent.TileSize.X, mapComponent.TileSize.Y)
                        )
                    ),
                    Behavior = CollisionBehavior.Wall
                })
                .Add(new RenderComponent(RenderingType.ColliderDebugView))
                .End();
            ref var collider = ref _staticColliders.Get(wallEntityId);
            collider.Box.Id = wallEntityId;
        }

    }
    
    private void CreateSquare()
    {
        _builder.Create()
            .Add(new TransformComponent{
                Location = new Vector2(0, 0.3f),
                Rotation = new Vector2(1.0f, 0.0f)
            })
            .Add(new MovementComponent()
            {
                Speed = 1.0f,
                Velocity = new Vector2(0.5f, 0.5f)
            })
            .Add(new RenderComponent(RenderingType.DebugUnitView))
            .End();
    }
    
    private void CreateItem()
    {
        int entityId = _itemsFactory.CreateItemEntity(0, 100);
        
        ref DynamicCollider collider = ref _dynamicColliders.Get(entityId);
        collider.Box = new Box(entityId, new Rectangle(
            Vector2.Zero,
            Vector2.One * 0.4f * 0.1f  / 2
        ));
        collider.Behavior = CollisionBehavior.Item;
    }
}