using UnityEngine;

public abstract class GamePausedState : GameState
{
    CursorLockMode _previousLockMode;
    private float _timeScaleBeforePause;
    
    public override void EnterState(StateMachine stateMachine)
    {
        base.EnterState(stateMachine);
        
        Pause();
    }
    
    public override void ExitState()
    {
        UnPause();
    }
    
    protected virtual void Pause()
    {
        _previousLockMode = Cursor.lockState;
        Cursor.lockState = CursorLockMode.None;

        _timeScaleBeforePause = Time.timeScale;
        Time.timeScale = 0.0f;
    }

    protected virtual void UnPause()
    {
        Cursor.lockState = _previousLockMode;
        Time.timeScale = _timeScaleBeforePause;
    }
}
