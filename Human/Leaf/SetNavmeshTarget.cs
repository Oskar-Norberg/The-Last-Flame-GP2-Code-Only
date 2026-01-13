using UnityEngine;
using UnityEngine.AI;
using Object = System.Object;

public class SetNavmeshTarget : Leaf
{
    private NavMeshAgent _navMeshAgent;

    public SetNavmeshTarget(NavMeshAgent navMeshAgent)
    {
        _navMeshAgent = navMeshAgent;
    }

    public override ReturnValue Evaluate()
    {
        if (!BlackBoard.TryGetData("TargetPosition", out Object targetPositionValue))
        {
            Debug.LogWarning("TargetPosition is not set.");
        }
        
        Vector3 targetPosition = (Vector3) targetPositionValue;
        
        _navMeshAgent.SetDestination(targetPosition);
        return ReturnValue.Success;
    }
}
