using MysticEchoes.Core.Base.ECS;
using MysticEchoes.Core.MapModule;
using SharpGL;

namespace MysticEchoes.Core;

public class Game
{
    private readonly IMazeGenerator _mazeGenerator;
    private readonly List<ExecutableSystem> _systems;
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
        _systems = systems.ToList();
        _entityFactory = entityFactory;
        _world = world;
    }

    public void Initialize(OpenGL gl)
    {
        _gl = gl;

        CreateTiles();
    }

    public void CreateTiles()
    {
        var map = _mazeGenerator.Generate();
        var maze = map.Maze;

        double tileWidth = 2d / maze.Size.Width;
        double tileHeight = 2d / maze.Size.Width;

        // for (var i = 0; i < maze.Size.Height; i++)
        // {
        //     for (var j = 0; j < maze.Size.Width; j++)
        //     {
        //         var tile = _entityFactory.Create();
        //
        //         tile.RenderStrategy = RenderingType.Tile;
        //
        //         var tileComponent = new TileComponent(
        //             maze.Cells[i, j],
        //             new Rectangle(
        //                 new Point(i * tileWidth, j*tileHeight),
        //                 new Size(tileWidth, tileHeight))
        //             );
        //         tile.AddComponent(tileComponent);
        //     }
        // }
    }

    public void Update()
    {
    }

    public void Render()
    {
        _gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
        _gl.LoadIdentity();
        _gl.Ortho(0, 2, 0, 2, -1, 1);

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