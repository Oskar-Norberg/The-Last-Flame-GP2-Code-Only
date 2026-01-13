using General.Player;
using UnityEngine;

public class IsWithinRangeToPlayer : Decorator
{
    private Transform _origin;
    private float _minRange, _maxRange;
    
    public IsWithinRangeToPlayer(Transform origin, float minRange, float maxRange, Node child) : base(child)
    {
        _origin = origin;
        _minRange = minRange;
        _maxRange = maxRange;
    }

    public override ReturnValue Evaluate()
    {
        float distanceToPlayer = Vector3.Distance(_origin.position, Player.Instance.transform.position);

        
        if (distanceToPlayer > _minRange && distanceToPlayer < _maxRange)
            return Child.Evaluate();

        return ReturnValue.Failure;
    }
}
