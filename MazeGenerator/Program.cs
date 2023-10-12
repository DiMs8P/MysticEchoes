namespace MazeGeneration;

internal class Program
{
    static void Main(string[] args)
    {
        var generator = new MazeGenerator(GenerationConfig.Default);

        var result = generator.Generate();
        PrintMaze(result.Maze);
    }

    public static void PrintMaze(Maze maze)
    {
        for (var i = 0; i < maze.Size.Height; i++)
        {
            for (var j = 0; j < maze.Size.Width; j++)
            {
                var cell = maze.Cells[i, j];
                if (cell == CellType.Empty)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                }
                else if (cell == CellType.FragmentBound)
                {
                    Console.BackgroundColor = ConsoleColor.Cyan;
                }
                else if (cell == CellType.Wall)
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                }
                else if (cell == CellType.Hall)
                {
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                }
                Console.Write('#');
                Console.ResetColor();
            }
            Console.WriteLine();
        }

    }
}