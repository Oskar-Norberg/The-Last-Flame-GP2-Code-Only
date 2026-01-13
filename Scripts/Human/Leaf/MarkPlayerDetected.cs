public class MarkPlayerDetected : Leaf
{
    public override ReturnValue Evaluate()
    {
        BlackBoard.SetData("IsPlayerDetected", true);
        
        return ReturnValue.Success;
    }
}
