using System.Numerics;
using MazeGeneration;

namespace MysticEchoes.Core.MapModule;

public struct TileMapComponent
{
    public Map Map { get; }
    public Vector2 TileSize { get; }

    public TileMapComponent(Map map)
    {
        Map = map;
        TileSize = new Vector2(
            2f / map.Size.Width,
            2f / map.Size.Height
        );
    }
}