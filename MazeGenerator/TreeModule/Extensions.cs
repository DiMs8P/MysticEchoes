using System.Drawing;

namespace MazeGeneration.TreeModule;

public static class Extensions
{
    public static bool ContainsNotStrict(this Rectangle rect, Point p)
    {
        return rect.X <= p.X && p.X <= rect.X + rect.Width
            && rect.Y <= p.Y && p.Y <= rect.Y + rect.Height;
    }
}