namespace MysticEchoes.Core.Base.Geometry;

public class Rectangle
{
    private readonly Point _leftBottom;
    private readonly Size _size;

    public Rectangle(Point leftBottom, Size size)
    {
        _leftBottom = leftBottom;
        _size = size;
    }
}