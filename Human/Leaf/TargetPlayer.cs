using General.Player;

public class TargetPlayer : Leaf
{
    public override ReturnValue Evaluate()
    {
        BlackBoard.SetData("TargetPosition", Player.Instance.transform.position);
        BlackBoard.SetData("TargetRotation", Player.Instance.transform);
        
        return ReturnValue.Success;
    }
}
