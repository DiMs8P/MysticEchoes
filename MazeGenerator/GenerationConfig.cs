using System.Drawing;

namespace MazeGeneration;

public class GenerationConfig
{
    public Size MinNodeSize { get; set; }
    public Size MinRoomSize { get; set; }
    public Size MinRoomPadding { get; set; }
    public Size MazeSize { get; set; }
    public GenerationRandom Random { get; set; }
    public double MaxWidthToHeightProportion { get; set; }
    public double MaxHeightToWidthProportion { get; set; }
    public double MaxDivideShift { get; set; }
    public int ThreeDepth { get; set; }

    public static GenerationConfig Default =>
        new()
        {
            MinNodeSize = new (10, 10),
            MinRoomSize = new (5, 5),
            MinRoomPadding = new (3, 3),
            MazeSize = new (120, 50),
            Random = new GenerationRandom(1353452337),
            MaxHeightToWidthProportion = 1/4d,
            MaxWidthToHeightProportion = 5d,
            MaxDivideShift = 0.1,
            ThreeDepth = 3
        };
}