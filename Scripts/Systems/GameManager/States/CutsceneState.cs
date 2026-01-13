using UnityEngine;

public class CutsceneState : GameRunningState
{
    private bool _update = true;
    
    public override void EnterState(StateMachine stateMachine)
    {
        base.EnterState(stateMachine);

        CutsceneManager.Instance.onCutsceneFinished += TransitionToPlayingState;
        
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    public override void ExitState()
    {
        Cursor.lockState = CursorLockMode.None;
    }
    
    public override void UpdateState(float deltaTime)
    {
        if (_update)
            base.UpdateState(deltaTime);
        
        if (Input.GetKeyDown(KeyCode.Y))
            CutsceneManager.Instance.SkipCutscene();
    }

    public override void FixedUpdateState(float fixedDeltaTime)
    {
        if (_update)
            base.FixedUpdateState(fixedDeltaTime);
    }
    
    public void StartSuppressUpdates()
    {
        InternalSuppressUpdates(true);
    }

    public void StopSuppressUpdates()
    {
        InternalSuppressUpdates(false);
    }

    private void InternalSuppressUpdates(bool suppress)
    {
        _update = !suppress;
    }

    private void TransitionToPlayingState()
    {
        StateMachine.SwitchState<PlayingState>();
    }
}
