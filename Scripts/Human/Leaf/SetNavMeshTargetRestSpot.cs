using UnityEngine;
using UnityEngine.AI;
using Object = System.Object;

public class SetNavMeshTargetRestSpot : Leaf
{
    private NavMeshAgent _navMeshAgent;
    
    public SetNavMeshTargetRestSpot(NavMeshAgent navMeshAgent)
    {
        _navMeshAgent = navMeshAgent;
    }
    
    public override ReturnValue Evaluate()
    {
        if (!BlackBoard.TryGetData("RestSpot", out Object restSpotValue))
        {
            Debug.LogWarning("RestSpot is not set.");
            return ReturnValue.Failure;
        }
        
        HumanRestSpot restSpot = restSpotValue as HumanRestSpot;

        if (restSpot == null)
        {
            Debug.LogWarning("RestSpot is not set.");

        }
        
        _navMeshAgent.SetDestination(restSpot.transform.position);
        return ReturnValue.Success;
    }
}
