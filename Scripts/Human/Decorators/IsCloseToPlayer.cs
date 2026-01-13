using General.Player;
using UnityEngine;

public class IsCloseToPlayer : Decorator
{
    private Transform _origin;
    private float _detectionRange;
    
    public IsCloseToPlayer(Transform origin, float detectionRange, Node child) : base(child)
    {
        _origin = origin;
        _detectionRange = detectionRange;
    }

    public override ReturnValue Evaluate()
    {
        float distanceToPlayer = Vector3.Distance(_origin.position, Player.Instance.transform.position);

        if (distanceToPlayer > _detectionRange)
            return ReturnValue.Failure;

        return Child.Evaluate();
    }
}
