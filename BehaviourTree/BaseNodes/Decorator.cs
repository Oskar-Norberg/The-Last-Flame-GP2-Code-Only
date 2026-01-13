
public abstract class Decorator : Node
{
    protected Node Child;

    protected Decorator(Node child)
    {
        Child = child;
    }

    public abstract override ReturnValue Evaluate();
    
    public override void PopulateBlackBoard(BlackBoard blackBoard)
    {
        base.PopulateBlackBoard(blackBoard);
        
        Child.PopulateBlackBoard(blackBoard);
    }
}

