namespace MazeGeneration;

internal class Program
{
    private static void Main(string[] args)
    {
        var generator = new MapGenerator(GenerationConfig.GetDefault());

        var result = generator.Generate();
    }
}