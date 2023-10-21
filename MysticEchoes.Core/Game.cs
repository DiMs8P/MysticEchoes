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
        mapEntity.AddComponent(new TileMapComponent(map.Maze))
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

        // foreach (var entity in _entities.Where(entity => entity.RenderStrategy is not RenderingType.None))
        // {
        //     if (entity.RenderStrategy is not RenderingType.Tile) continue;
        //
        //     var tile = entity.GetComponent<TileComponent>();
        //     var rect = tile.Rect;
        //
        //     _gl.Begin(OpenGL.GL_TRIANGLE_FAN);
        //
        //     Span<double> color = tile.Type switch
        //     {
        //         CellType.Empty => stackalloc double[] { 0.5d, 0.5d, 0.5d },
        //         CellType.FragmentBound => stackalloc double[] { 1d, 1d, 1d },
        //         CellType.Hall => stackalloc double[] { 0.8d, 0.8d, 0.1d },
        //         CellType.Wall => stackalloc double[] { 0.1d, 0.1d, 0.8d },
        //         _ => throw new ArgumentOutOfRangeException()
        //     };
        //
        //     _gl.Color(color[0], color[1], color[2]);
        //     _gl.Vertex(rect.LeftBottom.X, rect.LeftBottom.Y);
        //     _gl.Vertex(rect.LeftBottom.X, rect.LeftBottom.Y + rect.Size.Height);
        //     _gl.Vertex(rect.LeftBottom.X + rect.Size.Width, rect.LeftBottom.Y + rect.Size.Height);
        //     _gl.Vertex(rect.LeftBottom.X + rect.Size.Width, rect.LeftBottom.Y);
        //     _gl.End();
        // }
    }
}