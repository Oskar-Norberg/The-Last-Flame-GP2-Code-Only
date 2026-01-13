public class BehaviourTree
{
    private Node _rootNode;
    public BlackBoard BlackBoard { get; }

    public BehaviourTree(Composite rootNode)
    {
        BlackBoard = new BlackBoard();
        
        _rootNode = rootNode;
        _rootNode.PopulateBlackBoard(BlackBoard);
    }

    public void UpdateTree()
    {
        _rootNode.Evaluate();
    }
}
