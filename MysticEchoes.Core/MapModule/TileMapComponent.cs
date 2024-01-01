using System.Numerics;
using MazeGeneration;
using MysticEchoes.Core.Base.Geometry;

namespace MysticEchoes.Core.MapModule;

public struct TileMapComponent
{
    public Map Tiles { get; }
    public Vector2 TileSize { get; }

    public TileMapComponent(Map tiles)
    {
        Tiles = tiles;
        TileSize = new Vector2(
            2f / tiles.Size.Width,
            2f / tiles.Size.Width
        );
    }
}