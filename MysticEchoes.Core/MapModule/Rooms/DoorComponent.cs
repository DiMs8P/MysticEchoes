using MazeGeneration.TreeModule.Rooms;
using MysticEchoes.Core.Base.Geometry;
using Point = System.Drawing.Point;

namespace MysticEchoes.Core.MapModule.Rooms;

public struct DoorComponent
{
    public bool IsOpen { get; set; }
    public DoorOrientation Orientation { get; set; }
    public Point Tile { get; set; }
    public Rectangle Shape { get; set; }
}