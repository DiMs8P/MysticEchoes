namespace MysticEchoes.Core.Base.Geometry;

public class Rectangle
{
    public Point LeftBottom;
    public Size Size;

    public Rectangle(Point leftBottom, Size size)
    {
        LeftBottom = leftBottom;
        Size = size;
    }
}