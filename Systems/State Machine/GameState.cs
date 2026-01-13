using UnityEngine;

public abstract class GameState : MonoBehaviour
{
    protected StateMachine StateMachine;
    
    public virtual void EnterState(StateMachine stateMachine)
    {
        StateMachine = stateMachine;
        
        
    }
    
    public abstract void ExitState();
    
    public abstract void UpdateState(float deltaTime);
    
    public abstract void FixedUpdateState(float fixedDeltaTime);
}
