using General.Item;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class PlayingState : GameRunningState
{
    private InputAction _actEscape;
    private InputAction _actCodex;
    
    public override void EnterState(StateMachine stateMachine)
    {
        base.EnterState(stateMachine);

        // TODO: I don't think this is best practice, can someone who knows input system help me with this later? - Oskar
        _actEscape = InputSystem.actions.FindAction("Escape");
        _actCodex = InputSystem.actions.FindAction("Codex");
        
        if (_actEscape == null)
            Debug.LogWarning("PlayingState has no reference to the Escape action.");
        
        if (_actCodex == null)
            Debug.LogWarning("PlayingState has no reference to the Codex action.");

        Cursor.lockState = CursorLockMode.Locked;

        CutsceneManager.Instance.onCutsceneStart += TransitionToCutsceneState;
        Artifact.OnArtifactPickedUp += ArtifactPickedUp;
    }
    
    public override void ExitState()
    {
        CutsceneManager.Instance.onCutsceneStart -= TransitionToCutsceneState;
        Artifact.OnArtifactPickedUp -= ArtifactPickedUp;
        
        Cursor.lockState = CursorLockMode.None;
    }
    
    public override void UpdateState(float deltaTime)
    {
        base.UpdateState(deltaTime);
        
        if (_actEscape != null && _actEscape.WasPressedThisFrame())
            TransitionToPausedState();
        
        if (_actCodex != null &&  _actCodex.WasPressedThisFrame())
            TransitionToCodexState();
    }

    private void TransitionToCutsceneState(PlayableDirector playableDirector)
    {
        StateMachine.SwitchState<CutsceneState>();
    }
    
    private void TransitionToPausedState()
    {
        StateMachine.SwitchState<PausedState>();
    }

    private void ArtifactPickedUp(Artifact artifact)
    {
        TransitionToCodexState();
    }
    
    private void TransitionToCodexState()
    {
        StateMachine.SwitchState<CodexState>();
    }
}
