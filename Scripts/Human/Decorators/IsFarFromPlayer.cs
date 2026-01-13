using General.Player;
using UnityEngine;

public class IsFarFromPlayer : Decorator
{
    private Transform _origin;
    private float _range;
    
    public IsFarFromPlayer(Transform origin, float range, Node child) : base(child)
    {
        _origin = origin;
        _range = range;
    }

    public override ReturnValue Evaluate()
    {
        float distanceToPlayer = Vector3.Distance(_origin.position, Player.Instance.transform.position);

        if (distanceToPlayer > _range)
            return Child.Evaluate();
        
        return ReturnValue.Failure;
    }
}