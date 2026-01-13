using UnityEngine;

public class IsRestSpotFound : Decorator
{
    public IsRestSpotFound(Node child) : base(child)
    {
    }

    public override ReturnValue Evaluate()
    {
        if (!BlackBoard.TryGetData("FoundRestSpot", out object foundRestSpotValue))
        {
            return ReturnValue.Failure;
        }

        bool foundRestSpot = (bool) foundRestSpotValue;

        if (!foundRestSpot)
            return ReturnValue.Failure;

        return Child.Evaluate();
    }
}
