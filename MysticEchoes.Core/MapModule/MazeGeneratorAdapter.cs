using MazeGeneration;

namespace MysticEchoes.Core.MapModule;

public class MazeGeneratorAdapter : IMazeGenerator
{
    private readonly MapGenerator _mapGenerator;

    public MazeGeneratorAdapter()
    {
        _mapGenerator = new MapGenerator(GenerationConfig.Default);
    }

    public Map Generate()
    {
        return _mapGenerator.Generate();
    }
}