using System.Numerics;
using Leopotam.EcsLite;
using MysticEchoes.Core.Base.Geometry;
using MysticEchoes.Core.Collisions;
using MysticEchoes.Core.MapModule;
using MysticEchoes.Core.Movement;
using MysticEchoes.Core.Rendering;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Scene;

public class InitEnvironmentSystem : IEcsInitSystem
{
    [EcsInject] private IMazeGenerator _mazeGenerator;
    [EcsInject] private EntityFactory _factory;
    private EcsPool<StaticCollider> _staticColliders;

    public void Init(IEcsSystems systems)
    {
        var world = systems.GetWorld();
        _staticColliders = world.GetPool<StaticCollider>();

        CreateTiles();

        //CreateSquare();
    }

    private void CreateTiles()
    {
        var map = _mazeGenerator.Generate();

        var mapComponent = new TileMapComponent(map);
        var mapEntity = _factory.Create()
            .Add(mapComponent)
            .Add(new RenderComponent(RenderingType.TileMap))
            .End();

        foreach (var wall in map.WallTiles)
        {
            var wallEntityId = _factory.Create()
                .Add(new StaticCollider
                {
                    Box = new Box(
                        0,
                        new Rectangle(
                            new Vector2(wall.X * mapComponent.TileSize.X, wall.Y * mapComponent.TileSize.Y),
                            new Vector2(mapComponent.TileSize.X, mapComponent.TileSize.Y)
                        )
                    )
                })
                .Add(new RenderComponent(RenderingType.ColliderDebugView))
                .End();
            ref var collider = ref _staticColliders.Get(wallEntityId);
            collider.Box = collider.Box with {Id = wallEntityId};
        }

    }
    
    private void CreateSquare()
    {
        _factory.Create()
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
}