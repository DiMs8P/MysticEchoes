namespace MazeGeneration.TreeModule;

public class Tree<TNode>
    where TNode : ITreeNode<TNode>
{
    public TNode Root { get; }

    public Tree(TNode root)
    {
        Root = root;
    }

    public IEnumerable<TNode> DeepCrawl()
    {
        return DeepCrawl(Root);
    }

    public IEnumerable<TNode> DeepCrawl(TNode startNode)
    {
        var nodeStack = new Stack<TNode>();
        nodeStack.Push(startNode);

        void PushIfExist(TNode? node, int depth)
        {
            if (node is null) return;

            node.Depth = depth;
            nodeStack.Push(node);
        }

        while (nodeStack.Count > 0)
        {
            var node = nodeStack.Pop();

            yield return node;

            PushIfExist(node.LeftChild, node.Depth + 1);
            PushIfExist(node.RightChild, node.Depth + 1);
        }
    }
}