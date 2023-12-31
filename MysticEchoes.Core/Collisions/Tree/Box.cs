using MysticEchoes.Core.Base.Geometry;

namespace MysticEchoes.Core.Collisions.Tree;

public class Box
{
    public int Id { get; set; }
    public Rectangle Shape { get; set; }

    public Box(int id, Rectangle shape)
    {
        Id = id;
        Shape = shape;
    }
}