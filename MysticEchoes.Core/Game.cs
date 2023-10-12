using MysticEchoes.Core.MapModule;

namespace MysticEchoes.Core;

public class Game
{
    private readonly IMazeGenerator _mazeGenerator;

    public Game(IMazeGenerator mazeGenerator)
    {
        _mazeGenerator = mazeGenerator;
    }

    public void Run()
    {
        var map = _mazeGenerator.Generate();
        
    }

}