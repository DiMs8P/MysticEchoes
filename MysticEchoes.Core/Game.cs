using System.Diagnostics;
using System.Numerics;
using MysticEchoes.Core.Base.ECS;
using MysticEchoes.Core.MapModule;
using MysticEchoes.Core.Movement;
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
    private readonly SystemExecutionContext _systemExecutionContext;
    private readonly Stopwatch _updateTimer;

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

        _systemExecutionContext = new SystemExecutionContext();
        _updateTimer = new Stopwatch();
    }

    public void Initialize(OpenGL gl)
    {
        _gl = gl;

        _renderSystem.InitGl(gl);

        CreateTiles();

        _entityFactory.Create("square")
            .AddComponent(new TransformComponent()
            {
                Position = new Vector2(0, 0.3f),
                Velocity = new Vector2(0.04f, 0.02f)
            })
            .AddComponent(new RenderComponent(RenderingType.DebugUnitView));
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
        // _updateTimer.Stop();
        _systemExecutionContext.DeltaTime = _updateTimer.ElapsedMilliseconds / 1e3f;
        _updateTimer.Restart();
        foreach (var system in _systems)
        {
            system.Execute(_systemExecutionContext);
        }


        // _updateTimer.Start();
    }

    public void Render()
    {
        _renderSystem.Execute(_systemExecutionContext);
    }
}