using UnityEngine;

public class TargetRestSpot : Leaf
{
    private Transform _transform;
    
    public TargetRestSpot(Transform transform) : base()
    {
        _transform = transform;
    }
    
    public override ReturnValue Evaluate()
    {
        if (!BlackBoard.TryGetData("ClosestRestSpace", out object closestRestSpaceValue))
        {
            Debug.LogWarning("ClosestRestSpace not set");
            return ReturnValue.Failure;
        }

        HumanRestSpace closestRestSpace = closestRestSpaceValue as HumanRestSpace;

        if (!closestRestSpace)
        {
            Debug.LogWarning("ClosestRestSpace not set");
            return ReturnValue.Failure;
        }

        HumanRestSpot humanRestSpot = closestRestSpace.GetRandomRestSpot(_transform);

        if (!humanRestSpot)
        {
            Debug.LogWarning("Could not find closest rest spot");
            return ReturnValue.Failure;
        }
        
        BlackBoard.SetData("RestSpot", humanRestSpot);
        BlackBoard.SetData("FoundRestSpot", true);
        
        return ReturnValue.Success;
    }
}
