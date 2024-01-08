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
                    [EnemyType.Common] = 40,
                    [EnemyType.Elite] = 50,
                    [EnemyType.MiniBoss] = 60,
                },
                frequencies: new()
                {
                    [EnemyType.Common] = 11,
                    [EnemyType.Elite] = 3,
                    [EnemyType.MiniBoss] = 2,
                },
                new Dictionary<EnemyType, Point>
                {
                    [EnemyType.Common] = new (1, 1),
                    [EnemyType.Elite] = new(2, 2),
                    [EnemyType.MiniBoss] = new(3, 3),
                }),
        };
    // Пересекающиеся дороги
    //MinNodeSize = new (20, 20),
    //MinRoomSize = new (7, 7),
    //MinRoomPadding = new (2,2),
    //MazeSize = new (100, 100),
    //Random = new GenerationRandom(77551),
    //MaxHeightToWidthProportion = 1.6,
    //MaxWidthToHeightProportion = 1.6,
    //MaxDivideShift = 0.15,
    //ThreeDepth = 4

    // Пересекающиеся дороги, дороги сходятся к одному углу комнаты (как
    // потом надо будет делать дверь в углу??? мб просто убрать возможность
    // дорогам заканчиваться в углу комнаты)
    //MinNodeSize = new (20, 20),
    //MinRoomSize = new (7, 7),
    //MinRoomPadding = new (2,2),
    //MazeSize = new (100, 100),
    //Random = new GenerationRandom(1),
    //MaxHeightToWidthProportion = 1.6,
    //MaxWidthToHeightProportion = 1.6,
    //MaxDivideShift = 0.15,
    //ThreeDepth = 4

    // Дорога проходит через другую комнату!!!
    //MinNodeSize = new (20, 20),
    //MinRoomSize = new (7, 7),
    //MinRoomPadding = new (2,2),
    //MazeSize = new (100, 100),
    //Random = new GenerationRandom(5),
    //MaxHeightToWidthProportion = 1.6,
    //MaxWidthToHeightProportion = 1.6,
    //MaxDivideShift = 0.15,
    //ThreeDepth = 4

    // Комната вылазит ноду (вероятно проблема с тем, что
    // у встроенного класса rectangle в методе Contains
    // не включается верхняя граница при сравнении, заменить его на метод
    // ContainsNotStrict из Extension класса)
    // Дорога проходит сквозь другую комнату
    // MinNodeSize = new (10, 10),
    // MinRoomSize = new (4, 4),
    // MinRoomPadding = new (1,1),
    // MazeSize = new (50, 50),
    // Random = new GenerationRandom(7354543),
    // MaxHeightToWidthProportion = 1.4,
    // MaxWidthToHeightProportion = 2.1,
    // MaxDivideShift = 0,
    // ThreeDepth = 4
}