using MysticEchoes.Core.Base.Geometry;

namespace MysticEchoes.Core.MapModule.Rooms;

public struct RoomComponent
{
    public Rectangle Bound { get; set; }
    public List<int> Doors { get; set; }
}