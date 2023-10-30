using MazeGeneration;

namespace MysticEchoes.Core.MapModule;

public interface IMazeGenerator
{
    public MazeGenerationResult Generate();
}