using System.Drawing;

namespace MazeGeneration.TreeModule;

public class Hall
{
    public List<Point> ControlPoints { get; }
    public Rectangle StartRoom { get; }
    public Rectangle EndRoom { get; }

    public Hall(List<Point> controlPoints, Rectangle startRoom, Rectangle endRoom)
    {
        ControlPoints = controlPoints;
        StartRoom = startRoom;
        EndRoom = endRoom;
    }
}