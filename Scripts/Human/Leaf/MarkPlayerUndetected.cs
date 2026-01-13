public class MarkPlayerUndetected : Leaf
{
    public override ReturnValue Evaluate()
    {
        BlackBoard.SetData("IsPlayerDetected", false);

        return ReturnValue.Success;
    }
}
