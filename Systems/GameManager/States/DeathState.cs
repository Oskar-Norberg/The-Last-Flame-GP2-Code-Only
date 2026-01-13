using UnityEngine;

public class DeathState : GamePausedState
{
    [SerializeField] private GameObject deathPanel;
    
    public override void EnterState(StateMachine stateMachine)
    {
        base.EnterState(stateMachine);
        
        // Do Death state stuff
        deathPanel.SetActive(true);
    }
    
    public override void ExitState()
    {
    }

    public override void UpdateState(float deltaTime)
    {
    }

    public override void FixedUpdateState(float fixedDeltaTime)
    {
    }
}
