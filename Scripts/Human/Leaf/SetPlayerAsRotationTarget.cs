using General.Player;
using UnityEngine;

public class SetPlayerAsRotationTarget : Leaf
{
    public override ReturnValue Evaluate()
    {
        BlackBoard.SetData("TargetRotation", Player.Instance.transform);
        return ReturnValue.Success;
    }
}
