using UnityEngine.AI;

public class SetNavMeshSpeed : Leaf
{
    private NavMeshAgent _navMeshAgent;
    private float _speed, _acceleration;
    
    public SetNavMeshSpeed(NavMeshAgent navMeshAgent, float speed, float acceleration) : base()
    {
        _navMeshAgent = navMeshAgent;
        _speed = speed;
        _acceleration = acceleration;
    }    
    public override ReturnValue Evaluate()
    {
        _navMeshAgent.speed = _speed;
        _navMeshAgent.acceleration = _acceleration;
        
        return ReturnValue.Success;
    }
}
