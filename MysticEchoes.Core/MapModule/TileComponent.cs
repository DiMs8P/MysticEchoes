using MazeGeneration;
using MysticEchoes.Core.Base;
using MysticEchoes.Core.Base.Geometry;

namespace MysticEchoes.Core.MapModule;

public class TileComponent : IComponent
{
    public CellType Type { get; }
    public Rectangle Rect { get; }

    public TileComponent(CellType type, Rectangle rect)
    {
        Type = type;
        Rect = rect;
    }
}