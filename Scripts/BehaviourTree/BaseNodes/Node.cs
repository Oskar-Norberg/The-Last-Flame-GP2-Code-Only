public abstract class Node
{
    protected BlackBoard BlackBoard;
    
    public enum ReturnValue
    {
        Success,
        Failure
    }

    public abstract ReturnValue Evaluate();

    public virtual void PopulateBlackBoard(BlackBoard blackBoard)
    {
        BlackBoard = blackBoard;
    }
}