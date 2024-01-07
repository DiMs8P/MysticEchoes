using MazeGeneration;
using Microsoft.Extensions.Configuration;

namespace MysticEchoes.Core.MapModule;

public class MazeGeneratorAdapter : IMazeGenerator
{
    private readonly MapGenerator _mapGenerator;

    public MazeGeneratorAdapter(IConfiguration configuration)
    {
        if (configuration.GetValue<bool>("randomSeed"))
        {
            var seed = DateTime.Now.Millisecond;
            _mapGenerator = new MapGenerator(GenerationConfig.GetDefault(seed));
        }
        else
        {
            _mapGenerator = new MapGenerator(GenerationConfig.GetDefault());
        }
    }

    public Map Generate()
    {
        return _mapGenerator.Generate();
    }
}