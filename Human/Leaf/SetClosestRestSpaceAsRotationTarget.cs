using UnityEngine;

public class SetClosestRestSpaceAsRotationTarget : Leaf
{
    public override ReturnValue Evaluate()
    {
        if (!BlackBoard.TryGetData("ClosestRestSpace", out object closestRestSpaceValue))
        {
            Debug.LogWarning("ClosestRestSpace not set");
            return ReturnValue.Failure;
        }
        
        HumanRestSpace closestRestSpace = closestRestSpaceValue as HumanRestSpace;

        if (!closestRestSpace) return ReturnValue.Failure;
        
        BlackBoard.SetData("TargetRotation", closestRestSpace.transform);
        return ReturnValue.Success;
    }
}
