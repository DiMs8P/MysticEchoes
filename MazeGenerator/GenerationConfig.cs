using System.Drawing;
using MazeGeneration.Enemies;

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
    public RandomWalkParameter RoomRandomWalkParameter { get; set; }
    public EnemySpawnsGeneratorParameter EnemySpawnsGenerator { get; set; }

    public static GenerationConfig Default =>
        new()
        {
            MinNodeSize = new(14, 10),
            MinRoomSize = new(4, 4),
            MinRoomPadding = new(2, 2),
            MazeSize = new(70, 50),
            Random = new GenerationRandom(7354543),
            MaxHeightToWidthProportion = 1.4,
            MaxWidthToHeightProportion = 1.4,
            MaxDivideShift = 0,
            ThreeDepth = 3,
            RoomRandomWalkParameter = new RandomWalkParameter(25, 17, false),
            EnemySpawnsGenerator = new EnemySpawnsGeneratorParameter(
                random: new GenerationRandom(7354543),
                costs: new()
                {
                    [EnemyType.Common] = 11,
                    [EnemyType.Elite] = 19,
                    [EnemyType.MiniBoss] = 37,
                },
                frequencies: new()
                {
                    [EnemyType.Common] = 13,
                    [EnemyType.Elite] = 7,
                    [EnemyType.MiniBoss] = 4,
                },
                new Dictionary<EnemyType, Point>
                {
                    [EnemyType.Common] = new (1, 1),
                    [EnemyType.Elite] = new(2, 2),
                    [EnemyType.MiniBoss] = new(2, 2),
                }),
        };
}