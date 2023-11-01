using MazeGeneration;
using MysticEchoes.Core.Base.ECS;
using MysticEchoes.Core.Base.Geometry;
using MysticEchoes.Core.MapModule;
using SharpGL;

namespace MysticEchoes.Core.Rendering;

public class RenderSystem : ExecutableSystem
{
    private OpenGL? _gl;

    private static readonly Dictionary<CellType, double[]> TileColors = new Dictionary<CellType, double[]>
    {
        [CellType.Empty] = new[] { 0.5d, 0.5d, 0.5d },
        [CellType.FragmentBound] = new[] { 1d, 1d, 1d },
        [CellType.Hall] = new[] { 0.8d, 0.8d, 0.1d },
        [CellType.ControlPoint] = new[] { 0.8d, 0.1d, 0.1d },
        [CellType.Wall] = new[] { 0.1d, 0.1d, 0.8d }
    };

    public RenderSystem(World world)
        : base(world)
    {
    }

    public void InitGl(OpenGL gl)
    {
        _gl = gl;
    }

    public override void Execute()
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

                for (var i = 0; i < map.Tiles.Size.Height; i++)
                {
                    for (var j = 0; j < map.Tiles.Size.Width; j++)
                    {
                        var tileType = map.Tiles.Cells[i, j];
                        _gl.Begin(OpenGL.GL_TRIANGLE_FAN);

                        var color = TileColors[tileType];

                        var rect = new Rectangle(
                            new Point(i * map.TileSize.Width, j * map.TileSize.Height),
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

            }
            else if (rendering.Type is not RenderingType.None)
            {
                throw new NotImplementedException();
            }
        }
    }
}