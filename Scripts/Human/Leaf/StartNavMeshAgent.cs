using UnityEngine.AI;

public class StartNavMeshAgent : Leaf
{
    private NavMeshAgent _navMeshAgent;
    
    public StartNavMeshAgent(NavMeshAgent navMeshAgent)
    {
        _navMeshAgent = navMeshAgent;
    }
    
    public override ReturnValue Evaluate()
    {
        _navMeshAgent.isStopped = false;
        
        return ReturnValue.Success;
    }
}