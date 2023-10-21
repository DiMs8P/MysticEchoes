namespace MysticEchoes.Core.Base.Geometry;

public struct Rectangle
{
    public Point LeftBottom;
    public Size Size;

    public Rectangle(Point leftBottom, Size size)
    {
        LeftBottom = leftBottom;
        Size = size;
    }
}