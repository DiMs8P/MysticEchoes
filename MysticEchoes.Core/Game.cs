using MysticEchoes.Core.Base;
using MysticEchoes.Core.Base.Geometry;
using MysticEchoes.Core.MapModule;
using MysticEchoes.Core.Rendering;
using SharpGL;

namespace MysticEchoes.Core;

public class Game
{
    private readonly IMazeGenerator _mazeGenerator;
    private readonly List<ISystem> _systems;
    private readonly Renderer _renderer;
    private readonly EntityFactory _entityFactory;
    private readonly EntityPool _entities;
    private OpenGL _gl;
    private IDispatcher _dispatcher;
    private double y = 2d;

    public Game(
        IMazeGenerator mazeGenerator,
        IEnumerable<ISystem> systems,
        Renderer renderer,
        EntityFactory entityFactory,
        EntityPool entities
        )
    {
        _mazeGenerator = mazeGenerator;
        _systems = systems.ToList();
        _renderer = renderer;
        _entityFactory = entityFactory;
        _entities = entities;
    }

    public void Initialize(OpenGL gl, IDispatcher dispatcher)
    {
        _gl = gl;
        _dispatcher = dispatcher;

        CreateTiles();
        foreach (var entity in _entities)
        {
            entity.RenderStrategy?.InitGl(gl); 
        }
    }

    public void CreateTiles()
    {
        var map = _mazeGenerator.Generate();
        var maze = map.Maze;

        double tileWidth = 2d / maze.Size.Width;
        double tileHeight = 2d / maze.Size.Width;

        for (var i = 0; i < maze.Size.Height; i++)
        {
            for (var j = 0; j < maze.Size.Width; j++)
            {
                var tile = _entityFactory.Create<Tile>();

                var tileComponent = new TileComponent(
                    maze.Cells[i, j],
                    new Rectangle(
                        new Point(i * tileWidth, j*tileHeight),
                        new Size(tileWidth, tileHeight))
                    );
                tile.AddComponent(tileComponent);
            }
        }
    }

    public void Update()
    {
        // foreach (var system in _systems)
        // {
        //     foreach (var entity in _entities)
        //     {
        //         system.Update(entity);
        //     }
        // }
    }

    public void Render()
    {
        _gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
        _gl.LoadIdentity();
        _gl.Ortho(0, 2, 0, 2, -1, 1);

        

        y -= 0.003;

        foreach (var entity in _entities.Where(entity => entity.RenderStrategy is not null))
        {
            _renderer.AddInPool(entity.RenderStrategy!);
        }
        
        _renderer.DoRender();
    }
}