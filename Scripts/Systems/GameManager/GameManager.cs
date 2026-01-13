using System;
using General.Player;
using UnityEngine;

public class GameManager : StateMachine
{
    #region Singleton
    
    // TODO: See if this can inherit from the Singleton class instead of doing this duplicate code
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("Instance of " + nameof(GameManager) + " is null");
            }
            
            return _instance;
        }
        
        private set => _instance = value;
    }

    private static GameManager _instance;

    protected virtual void Awake()
    {
        base.Awake();
        
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
    }

    protected void OnApplicationQuit()
    {
        Instance = null;
        Destroy(gameObject);
    }
    #endregion

    public bool isPaused => currentState.GetType() == typeof(PausedState);
    
    public Type currentStateType => currentState.GetType();

    private void OnDestroy()
    {
        currentState?.ExitState();
    }
}
