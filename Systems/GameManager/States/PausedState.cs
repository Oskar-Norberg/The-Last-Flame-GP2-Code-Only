using UnityEngine;
using UnityEngine.InputSystem;

public class PausedState : GamePausedState
{
    [SerializeField] private MenuManager menuManager;
    [SerializeField] private GameObject pausePanel;
    
    private InputAction _actEscape;
    private InputAction _actCancel;
    

    private void Start()
    {
        if (!menuManager)
            Debug.LogWarning("PausedState has no reference to the MenuManager.");
        
        _actEscape = InputSystem.actions.FindAction("Escape");
        _actCancel = InputSystem.actions.FindAction("Cancel");
    }
    
    public override void EnterState(StateMachine stateMachine)
    {
        base.EnterState(stateMachine);
    }
    
    public override void UpdateState(float deltaTime)
    {
        if ((_actEscape.WasPressedThisFrame() || _actCancel.WasPressedThisFrame()) && pausePanel.activeSelf)
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
        
        menuManager.TogglePause(true);
        menuManager.onResumeButtonPressedEvent += TransitionToPlayingState;
    }
    
    protected override void UnPause()
    {
        base.UnPause();

        if (!menuManager) return;
        
        menuManager.TogglePause(false);
        menuManager.onResumeButtonPressedEvent -= TransitionToPlayingState;
    }
}
