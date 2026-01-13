using UnityEngine.AI;

public class StopNavMeshAgent : Leaf
{
    private NavMeshAgent _navMeshAgent;
    
    public StopNavMeshAgent(NavMeshAgent navMeshAgent)
    {
        _navMeshAgent = navMeshAgent;
    }
    
    public override ReturnValue Evaluate()
    {
        _navMeshAgent.isStopped = true;
        
        return ReturnValue.Success;
    }
}
