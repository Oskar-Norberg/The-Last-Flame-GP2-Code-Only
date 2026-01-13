using System.Collections.Generic;

public abstract class Composite : Node
{
    protected List<Node> Children;

    protected Composite(List<Node> children)
    {
        Children = children;
    }

    public abstract override ReturnValue Evaluate();
    
    public override void PopulateBlackBoard(BlackBoard blackBoard)
    {
        base.PopulateBlackBoard(blackBoard);
        
        foreach (Node child in Children)
        {
            child.PopulateBlackBoard(BlackBoard);
        }
    }
}
