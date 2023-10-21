using MazeGeneration;

namespace MysticEchoes.Core.MapModule;

public class MazeGeneratorAdapter : IMazeGenerator
{
    private readonly MazeGenerator _mazeGenerator;

    public MazeGeneratorAdapter()
    {
        _mazeGenerator = new MazeGenerator(GenerationConfig.Default);
    }

    public MazeGenerationResult Generate()
    {
        return _mazeGenerator.Generate();
    }
}