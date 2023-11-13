using MazeGeneration;
using MysticEchoes.Core.Base.ECS;
using MysticEchoes.Core.MapModule;
using MysticEchoes.Core.Movement;
using SharpGL;
using Point = MysticEchoes.Core.Base.Geometry.Point;
using Rectangle = MysticEchoes.Core.Base.Geometry.Rectangle;
using Size = MysticEchoes.Core.Base.Geometry.Size;


namespace MysticEchoes.Core.Rendering;

public class RenderSystem : ExecutableSystem
{
    private OpenGL? _gl;

    private static readonly Dictionary<CellType, double[]> TileColors = new()
    {
        [CellType.Empty] = new[] { 64d/255, 64d/255, 64d/255 },
        [CellType.FragmentBound] = new[] { 0d,0d,0d },
        [CellType.Hall] = new[] { 0.8d, 0.8d, 0.1d },
        [CellType.ControlPoint] = new[] { 0.8d, 0.1d, 0.1d },
        [CellType.Wall] = new[] { 103d/255, 65d/255, 72d/255 },
        [CellType.Floor] = new[] {53d/255,25d/255,48d/255}
    };

    public RenderSystem(World world)
        : base(world)
    {
    }

    public void InitGl(OpenGL gl)
    {
        _gl = gl;
    }

    public override void Execute(SystemExecutionContext _)
    {
        if (_gl is null)
            throw new InvalidOperationException("Open Gl wasn't initialized");

        _gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
        _gl.LoadIdentity();
        _gl.Ortho(0, 2, 0, 2, -1, 1);

        foreach (var rendering in World.GetAllComponents<RenderComponent>().Enumerate())
        {
            var entity = World.GetEntity(rendering.OwnerId);

            if (rendering.Type is RenderingType.TileMap)
            {
                var map = entity.GetComponent<TileMapComponent>();
                {
                    var color = TileColors[CellType.Empty];
                    _gl.Begin(OpenGL.GL_TRIANGLE_FAN);
                    _gl.Color(color[0], color[1], color[2]);
                    _gl.Vertex(0d, 0d);
                    _gl.Vertex(2d, 0d);
                    _gl.Vertex(2d, 2d);
                    _gl.Vertex(0d, 2d);
                    _gl.End();
                }
                
                foreach (var floor in map.Tiles.FloorTiles)
                {
                    _gl.Begin(OpenGL.GL_TRIANGLE_FAN);
                
                    var color = TileColors[CellType.Floor];
                
                    var rect = new Rectangle(
                        new Point(floor.X * map.TileSize.Width, floor.Y * map.TileSize.Height),
                        new Size(map.TileSize.Width, map.TileSize.Height)
                    );
                
                    _gl.Color(color[0], color[1], color[2]);
                    _gl.Vertex(rect.LeftBottom.X, rect.LeftBottom.Y);
                    _gl.Vertex(rect.LeftBottom.X, rect.LeftBottom.Y + rect.Size.Height);
                    _gl.Vertex(rect.LeftBottom.X + rect.Size.Width, rect.LeftBottom.Y + rect.Size.Height);
                    _gl.Vertex(rect.LeftBottom.X + rect.Size.Width, rect.LeftBottom.Y);
                    _gl.End();
                }
                foreach (var floor in map.Tiles.WallTiles)
                {
                    _gl.Begin(OpenGL.GL_TRIANGLE_FAN);

                    var color = TileColors[CellType.Wall];

                    var rect = new Rectangle(
                        new Point(floor.X * map.TileSize.Width, floor.Y * map.TileSize.Height),
                        new Size(map.TileSize.Width, map.TileSize.Height)
                    );

                    _gl.Color(color[0], color[1], color[2]);
                    _gl.Vertex(rect.LeftBottom.X, rect.LeftBottom.Y);
                    _gl.Vertex(rect.LeftBottom.X, rect.LeftBottom.Y + rect.Size.Height);
                    _gl.Vertex(rect.LeftBottom.X + rect.Size.Width, rect.LeftBottom.Y + rect.Size.Height);
                    _gl.Vertex(rect.LeftBottom.X + rect.Size.Width, rect.LeftBottom.Y);
                    _gl.End();
                }
            }
            else if (rendering.Type is RenderingType.DebugUnitView)
            {
                var transform = entity.GetComponent<TransformComponent>();

                _gl.Begin(OpenGL.GL_QUADS);
                _gl.Color(1f, 0f, 0f);

                const float halfSize = 0.05f;

                _gl.Vertex(transform.Position.X - halfSize, transform.Position.Y + halfSize);
                _gl.Vertex(transform.Position.X - halfSize, transform.Position.Y - halfSize);
                _gl.Vertex(transform.Position.X + halfSize, transform.Position.Y - halfSize);
                _gl.Vertex(transform.Position.X + halfSize, transform.Position.Y + halfSize);
                _gl.End();
            }
            else if (rendering.Type is not RenderingType.None)
            {
                throw new NotImplementedException();
            }
        }
    }
}