using MazeGeneration;
using MazeGeneration.TreeModule;
using MysticEchoes.Core.Base;

namespace MysticEchoes.Core.MapModule;

class Map : IComponent
{
    public Tree<RoomNode> Tree { get; }
    public Maze Maze { get; }

    public Map(MazeGenerationResult map)
    {
        Tree = map.Tree;
        Maze = map.Maze;
    }
}