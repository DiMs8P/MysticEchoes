namespace MazeGeneration;

internal class Program
{
    private static void Main(string[] args)
    {
        var generator = new MapGenerator(GenerationConfig.Default);

        var result = generator.Generate();
    }
}