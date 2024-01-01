using System.Diagnostics;
using System.Numerics;

namespace MysticEchoes.Core.Base.Geometry;

public struct Rectangle
{
    public Vector2 LeftBottom { get; set; }
    public Vector2 LeftTop => LeftBottom + Size with { X = 0 };
    public Vector2 RightBottom => LeftBottom + Size with { Y = 0 };
    public Vector2 RightTop => LeftBottom + Size;
    public float Left => LeftBottom.X;
    public float Right => LeftBottom.X + Size.X;
    public float Bottom => LeftBottom.Y;
    public float Top => LeftBottom.Y + Size.Y;
    public Vector2 Size { get; set; }

    public Rectangle(Vector2 leftBottom, Vector2 size)
    {
        LeftBottom = leftBottom;
        Size = size;
    }

    public bool Intersects(Rectangle other)
    {
        return !(Left > other.Right || Right < other.Left) 
            && !(Bottom > other.Top || Top < other.Bottom);
    }

    public bool Contains(Vector2 point)
    {
        return Left <= point.X && point.X <= Right
            && Bottom <= point.Y && point.Y <= Top;
    }

    public override string ToString()
    {
        return LeftBottom + "---" + RightTop;
    }
}