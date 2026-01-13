using General.Player;
using UnityEngine;

public class TargetAwayFromPlayer : Leaf
{
    private Transform _origin;
    private float _distanceFromPlayer;
    
    public TargetAwayFromPlayer(Transform origin, float distanceFromPlayer)
    {
        _origin = origin;
        _distanceFromPlayer = distanceFromPlayer;
    }
    
    public override ReturnValue Evaluate()
    {
        Vector3 directionToPlayer = Player.Instance.transform.position - _origin.transform.position;
        directionToPlayer.Normalize();

        Vector3 directionAwayFromPlayer = -directionToPlayer * _distanceFromPlayer;

        Vector3 targetPosition = _origin.transform.position + directionAwayFromPlayer;

        BlackBoard.SetData("TargetPosition", targetPosition);
        BlackBoard.SetData("TargetRotation", targetPosition);
        
        return ReturnValue.Success;
    }
}