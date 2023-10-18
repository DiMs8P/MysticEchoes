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

    public void Initialize(OpenGL gl)
    {
        CreateTiles();
        foreach (var entity in _entities)
        {
            entity.RenderStrategy?.InitGl(gl); 
        }
    }

    public void CreateTiles()
    {
        const double tileWidth = 1;
        const double tileHeight= 1;

        var map = _mazeGenerator.Generate();
        var maze = map.Maze;

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

    public void Run()
    {
        while (true)
        {
            Update();
            Render();
        }
    }

    private void Update()
    {
        foreach (var system in _systems)
        {
            foreach (var entity in _entities)
            {
                system.Update(entity);
            }
        }
    }

    private void Render()
    {
        foreach (var entity in _entities.Where(entity => entity.RenderStrategy is not null))
        {
            _renderer.AddInPool(entity.RenderStrategy!);
        }
        
        _renderer.DoRender();
    }
}