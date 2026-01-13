using System;
using General.Player;
using UnityEngine;

public abstract class GameRunningState : GameState
{
    private Player _player;
    private HumanManager _humanManager;

    private void Awake()
    {
        _player = Player.Instance;

        _humanManager = HumanManager.Instance;

    }

    public override void EnterState(StateMachine stateMachine)
    {
        base.EnterState(stateMachine);

        Player.Instance.PlayerEntity.OnDeath += TransitionToDeathState;
    }

    public override void ExitState()
    {
        Player.Instance.PlayerEntity.OnDeath -= TransitionToDeathState;
    }

    public override void UpdateState(float deltaTime)
    {
        if (_player)
            _player.CustomUpdate(deltaTime);
        if (_humanManager)
            _humanManager.CustomUpdate(deltaTime);
    }

    public override void FixedUpdateState(float fixedDeltaTime)
    {
        if (_player)
            _player.CustomFixedUpdate(fixedDeltaTime);
        if (_humanManager)
            _humanManager.CustomFixedUpdate(fixedDeltaTime);
    }

    private void TransitionToDeathState()
    {
        StateMachine.SwitchState<DeathState>();
    }
}