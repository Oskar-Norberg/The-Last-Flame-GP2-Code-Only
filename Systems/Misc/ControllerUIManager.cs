using System;
using General.Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ControllerUIManager : MonoBehaviour
{

    [SerializeField] private GameObject[] keyboardObjects;
    [SerializeField] private GameObject[] gamepadObjects;
    
    public enum Controller {Keyboard, Gamepad}
    public Controller LastInput { get; private set; }

    private void Start()
    {
    }

    void Update()
    {
        CheckController();
        
        foreach (var obj in keyboardObjects)
        {
            obj.SetActive(LastInput == Controller.Keyboard);
        }
        
        foreach (var obj in gamepadObjects)
        {
            obj.SetActive(LastInput == Controller.Gamepad);
        }

        // reset the selection if on keyboard
        if (LastInput == Controller.Keyboard)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
    
    private void CheckController()
    {
        if (Gamepad.current == null)
        {
            LastInput = Controller.Keyboard;
            return;
        }
            
        if (Gamepad.current.IsActuated(0))
        {
            LastInput = Controller.Gamepad;
        }
        else if (Keyboard.current.IsActuated(0))
        {
            LastInput = Controller.Keyboard;
        }
    }
}
