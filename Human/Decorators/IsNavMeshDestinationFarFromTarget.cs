using UnityEngine;
using UnityEngine.AI;

public class IsNavMeshDestinationFarFromTarget : Decorator
{
    private NavMeshAgent _agent;
    private float _distanceThreshold;
    
    public IsNavMeshDestinationFarFromTarget(NavMeshAgent agent, float distanceThreshold, Node child) : base(child)
    {
        _agent = agent;
        _distanceThreshold = distanceThreshold;
    }
    
    public override ReturnValue Evaluate()
    {
        if (!BlackBoard.TryGetData("TargetPosition", out object targetPositionValue))
            Debug.LogWarning("TargetPosition is not set.");
        
        Vector3 targetPosition = (Vector3) targetPositionValue;
        
        if (Vector3.Distance(_agent.destination, targetPosition) > _distanceThreshold)
            return Child.Evaluate();
        
        return ReturnValue.Failure;
    }
}
