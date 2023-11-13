using MazeGeneration;
using MysticEchoes.Core.Base.ECS;
using MysticEchoes.Core.Base.Geometry;

namespace MysticEchoes.Core.MapModule;

public class TileMapComponent : Component
{
    public Map Tiles { get; }
    public Size TileSize { get; }

    public TileMapComponent(Map tiles)
    {
        Tiles = tiles;
        TileSize = new Size(
            2d / tiles.Size.Width,
            2d / tiles.Size.Width
        );
    }
}