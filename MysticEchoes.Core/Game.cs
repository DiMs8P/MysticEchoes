using MysticEchoes.Core.Base.ECS;
using MysticEchoes.Core.MapModule;
using MysticEchoes.Core.Rendering;
using SharpGL;

namespace MysticEchoes.Core;

public class Game
{
    private readonly IMazeGenerator _mazeGenerator;
    private readonly List<ExecutableSystem> _systems;
    private readonly RenderSystem _renderSystem;
    private readonly EntityFactory _entityFactory;
    private readonly World _world;
    private OpenGL _gl;

    public Game(
        IMazeGenerator mazeGenerator,
        IEnumerable<ExecutableSystem> systems,
        EntityFactory entityFactory,
        World world
        )
    {
        _mazeGenerator = mazeGenerator;
        _systems = systems.Where(x => x.GetType() != typeof(RenderSystem))
            .ToList();
        _renderSystem = (RenderSystem)systems.First(x => x.GetType() == typeof(RenderSystem));
        _entityFactory = entityFactory;
        _world = world;
    }

    public void Initialize(OpenGL gl)
    {
        _gl = gl;

        _renderSystem.InitGl(gl);

        CreateTiles();
    }

    public void CreateTiles()
    {
        var map = _mazeGenerator.Generate();

        var mapEntity = _entityFactory.Create("map");
        mapEntity.AddComponent(new TileMapComponent(map))
            .AddComponent(new RenderComponent(RenderingType.TileMap));
    }

    public void Update()
    {
        foreach (var system in _systems)
        {
            system.Execute();
        }
    }

    public void Render()
    {
        _renderSystem.Execute();
    }
}