using System;
using UnityEngine;
using UnityEngine.UI;

public class Fullscreen : MonoBehaviour
{
    // [HideInInspector] public bool desiredToggleState;
    private Toggle toggleObject;
    // [SerializeField] private string toggleType;
    [SerializeField] private ToggleType toggleType;
    
    enum ToggleType
    {
        Fullscreen,
        HUD,
        Sprint,
        Subtitles
    }

    private void Awake()
    {
        toggleObject = GetComponent<Toggle>();
    }

    private void Start()
    {
        LoadToggleState();
    }

    private void LoadToggleState()
    {
        // Get the correct preview value from the settings
        bool loadedToggleState = true;
        
        switch (toggleType)
        {
            case ToggleType.Fullscreen:
                loadedToggleState = SettingsManager.Instance.gameSettings.generalSettings.fullscreenPreview;
                break;
            case ToggleType.HUD:
                loadedToggleState = SettingsManager.Instance.gameSettings.generalSettings.hudEnabledPreview;
                break;
            case ToggleType.Sprint:
                loadedToggleState = SettingsManager.Instance.gameSettings.generalSettings.sprintIsTogglePreview;
                break;
            case ToggleType.Subtitles:
                loadedToggleState = SettingsManager.Instance.gameSettings.generalSettings.subtitlesPreview;
                break;
        }
        
        toggleObject.isOn = loadedToggleState;
    }

    public void OnToggleValueChanged(bool toggleValue)
    {
        toggleObject.isOn = toggleValue;
        switch (toggleType)
        {
            case ToggleType.Fullscreen:
                SettingsManager.Instance.SetFullscreen(toggleValue);
                break;
            case ToggleType.HUD:
                SettingsManager.Instance.SetHUDEnabled(toggleValue);
                break;
            case ToggleType.Sprint:
                SettingsManager.Instance.SetSprintToggle(toggleValue);
                break;
            case ToggleType.Subtitles:
                SettingsManager.Instance.SetSubtitles(toggleValue);
                break;
        }
    }
}