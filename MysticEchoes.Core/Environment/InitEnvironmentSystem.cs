using System.Numerics;
using Leopotam.EcsLite;
using MysticEchoes.Core.MapModule;
using MysticEchoes.Core.Movement;
using MysticEchoes.Core.Rendering;
using SevenBoldPencil.EasyDi;

namespace MysticEchoes.Core.Environment;

public class InitEnvironmentSystem : IEcsInitSystem
{
    [EcsInject] private EntityFactory _factory;
    [EcsInject] private IMazeGenerator _mazeGenerator;
    public void Init(IEcsSystems systems)
    {
        CreateTiles();
        CreateSquare();
    }

    private void CreateTiles()
    {
        var map = _mazeGenerator.Generate();

        var mapEntity = _factory.Create()
            .Add(new TileMapComponent(map))
            .Add(new RenderComponent(RenderingType.TileMap))
            .End();
    }
    
    private void CreateSquare()
    {
        _factory.Create()
            .Add(new TransformComponent{
                Position = new Vector2(0, 0.3f),
                Velocity = new Vector2(0.04f, 0.02f)
            })
            .Add(new RenderComponent(RenderingType.DebugUnitView))
            .End();
    }
}