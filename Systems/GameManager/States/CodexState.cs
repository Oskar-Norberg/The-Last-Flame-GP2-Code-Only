using UnityEngine;
using UnityEngine.InputSystem;

public class CodexState : GamePausedState
{
    [SerializeField] private MenuManager menuManager;
    
    private InputAction _actCodex;
    private InputAction _actCancel;

    private void Start()
    {
        // if (!menuManager)
        //     Debug.LogWarning("CodexState has no reference to the Codex Menu.");
    }
    
    public override void EnterState(StateMachine stateMachine)
    {
        base.EnterState(stateMachine);
        
        menuManager.onResumeButtonPressedEvent += TransitionToPlayingState;
        
        _actCodex = InputSystem.actions.FindAction("Codex");
        _actCancel = InputSystem.actions.FindAction("Cancel");
    }

    public override void ExitState()
    {
        base.ExitState();
        menuManager.onResumeButtonPressedEvent -= TransitionToPlayingState;
    }
    
    public override void UpdateState(float deltaTime)
    {
        if ((_actCodex.WasPressedThisFrame() || _actCancel.WasPressedThisFrame()) && !menuManager.isToggling)
            TransitionToPlayingState();
    }

    public override void FixedUpdateState(float fixedDeltaTime)
    {
    }

    private void TransitionToPlayingState()
    {
        StateMachine.SwitchState<PlayingState>();
    }

    protected override void Pause()
    {
        base.Pause();
        
        if (!menuManager) return;
        
        menuManager.ToggleCodex(true);
        menuManager.onResumeButtonPressedEvent += TransitionToPlayingState;
    }
    
    protected override void UnPause()
    {
        base.UnPause();
    
        if (!menuManager) return;
        
        menuManager.ToggleCodex(false);
        menuManager.onResumeButtonPressedEvent -= TransitionToPlayingState;
    }
}
