using System.Drawing;

namespace MazeGeneration.TreeModule;

public class RoomNode : ITreeNode<RoomNode>
{
    public Point Position { get; set; }
    public Size Size { get; set; }
    public RoomNode? LeftChild { get; set; }
    public RoomNode? RightChild { get; set; }
    public TreeNodeType Type => LeftChild is null && RightChild is null
        ? TreeNodeType.Leaf
        : TreeNodeType.Node;
    public int Depth { get; set; }
    public Rectangle? Room { get; set; }
    public Hall? Hall { get; set; }

    public RoomNode(Point position, Size size)
    {
        Position = position;
        Size = size;
    }
}