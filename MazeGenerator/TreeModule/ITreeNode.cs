namespace MazeGeneration.TreeModule;

public interface ITreeNode<T>
    where T : ITreeNode<T>
{
    T? LeftChild { get; set; }
    T? RightChild { get; set; }
    TreeNodeType Type { get; }
    int Depth { get; set; }
}