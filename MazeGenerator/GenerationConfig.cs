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
            MinRoomSize = new (6, 6),
            MinRoomPadding = new (1,1),
            MazeSize = new (70, 70),
            Random = new GenerationRandom(1234),
            MaxHeightToWidthProportion = 1.6,
            MaxWidthToHeightProportion = 1.6,
            MaxDivideShift = 0.3,
            ThreeDepth = 3
        };
}