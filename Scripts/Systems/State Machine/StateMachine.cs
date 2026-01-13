using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    [SerializeField] private GameState startState;
    
    [SerializeField] private List<GameState> states = new();
    
    protected GameState currentState;

    protected void Awake()
    {
        SwitchState(startState);
    }

    public void SwitchState<TNextState>()
    {
        foreach (GameState state in states)
        {
            if (state.GetType() != typeof(TNextState)) continue;
            
            SwitchState(state);
            return;
        }
        
        Debug.Log("State not found");
    }
    
    private void SwitchState(GameState state)
    {
        currentState?.ExitState();
        currentState = state;
        state.EnterState(this);
    }

    private void Update()
    {
        currentState?.UpdateState(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        currentState?.FixedUpdateState(Time.fixedDeltaTime);
    }
}
