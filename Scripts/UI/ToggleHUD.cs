using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ToggleHUD : MonoBehaviour
{
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject pausePanel;
    
    private bool isHUDEnabled;
    
    [Header("Inputs")]
    [SerializeField] private InputActionAsset inputActionAsset;
    private InputAction toggleHUDAction;
    
    // public void OnEnable() {
    //     InputActionMap actionMap = inputActionAsset.FindActionMap("Player");
    //
    //     toggleHUDAction = actionMap.FindAction("ToggleHUD");
    //     toggleHUDAction.Enable();
    // }
    //
    // private void OnDisable() {
    //     toggleHUDAction.Disable();
    // }

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
            ToggleChoiceHUD(false);
        else 
            ToggleChoiceHUD(true);
    }

    void Update()
    {
        if (GameManager.Instance.currentStateType == typeof(PlayingState))
            ToggleChoiceHUD(true);
        // else 
        //     ToggleChoiceHUD(false);
        if (GameManager.Instance.currentStateType == typeof(CodexState) ||
            GameManager.Instance.currentStateType == typeof(DeathState) ||
            GameManager.Instance.currentStateType == typeof(PausedState))
        {
            ToggleChoiceHUD(false);
        }
    }

    private void HandleToggleHUD()
    {
        isHUDEnabled = !isHUDEnabled;
        hudPanel.SetActive(isHUDEnabled);
    }
    
    public void ToggleChoiceHUD(bool toggle)
    {
        isHUDEnabled = toggle;
        
        if (isHUDEnabled)
            hudPanel.SetActive(isHUDEnabled);
        else 
            hudPanel.SetActive(isHUDEnabled);
    }
}
