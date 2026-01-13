using UnityEngine;

public class IsPlayerDetected : Decorator
{
    public IsPlayerDetected(Node child) : base(child)
    {
    }

    public override ReturnValue Evaluate()
    {
        if (!BlackBoard.TryGetData("IsPlayerDetected", out object isPlayerDetectedValue))
            return ReturnValue.Failure;
        
        bool isPlayerDetected = (bool) isPlayerDetectedValue;

        if (isPlayerDetected)
            return Child.Evaluate();

        return ReturnValue.Failure;
    }
}
