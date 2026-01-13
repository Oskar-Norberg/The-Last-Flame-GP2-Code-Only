using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject titlePanel;         // press to start
    [SerializeField] private GameObject mainPanel;          // play game, options, quit
    [SerializeField] private GameObject playPanel;          // new game, load game, continue
    [SerializeField] private GameObject optionsPanel;       // general, sound, controls
    [SerializeField] private GameObject soundPanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject generalPanel;
    [SerializeField] private GameObject controlsOverlayPanel;           // to indicate back and select buttons to player on menus

    [Header("Buttons")]
    [SerializeField] private GameObject firstButtonMain;
    [SerializeField] private GameObject firstButtonOptions;
    [SerializeField] private GameObject firstButtonPlay;
    [SerializeField] private GameObject firstButtonSound;
    [SerializeField] private GameObject firstButtonGeneral;
    [SerializeField] private GameObject firstButtonControls;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button continueGameButton;

    [Header("Panel Fade")]
    [SerializeField] float fadeDuration = 1f;
    private bool isPastTitleScreen = false;

    [Header("Cameras")]
    [SerializeField] private CinemachineCamera mainCamera;
    [SerializeField] private CinemachineCamera optionsCamera;
    [SerializeField] private CinemachineCamera playGameCamera;
    [SerializeField] private CinemachineCamera titleCamera;
    [SerializeField] private CinemachineCamera transitionCamera;
    [SerializeField] private float blendTime = 2f;

    [Header("Adjustable Options")]
    [SerializeField] private Image masterVolumeSlider;
    [SerializeField] private Image sfxVolumeSlider;
    [SerializeField] private Image musicVolumeSlider;
    [SerializeField] private Image dialogueVolumeSlider;
    [SerializeField] private Image ambienceVolumeSlider;
    [SerializeField] private Fullscreen fullscreenToggle;

    [Header("Inputs")]
    [SerializeField] private InputActionAsset inputActionAsset;
    private InputAction cancelAction;
    private InputAction submitAction;
    private InputAction reselectAction;
    private InputAction clickAction;
    private InputAction action;

    private bool blocked = false;
    private Button[] allButtons;

    #region Input Handling

    private void OnEnable() {
        InputActionMap uiActionMap = inputActionAsset.FindActionMap("UI");

        cancelAction = uiActionMap.FindAction("Cancel");
        cancelAction.Enable();

        submitAction = uiActionMap.FindAction("Submit");
        submitAction.Enable(); 

        reselectAction = uiActionMap.FindAction("Navigate");
        reselectAction.Enable(); 
        
        clickAction = uiActionMap.FindAction("Click");
        clickAction.Enable();

        action = uiActionMap.FindAction("Action");
        action.Enable();
        action.performed += BlockInput;
    }

    private void OnDisable()
    {
        cancelAction.Disable();
        submitAction.Disable();
        reselectAction.Disable();
        clickAction.Disable();
        action.Disable();
    }
    
    private void Update()
    {
        // handle the title screen fade and camera pan (the start)
        if ((submitAction.triggered || clickAction.triggered) && !isPastTitleScreen)
        {
            StartCoroutine(FadeOutCoroutine(titlePanel));
            StartCoroutine(SwitchToNewCamera(titleCamera, mainCamera));
            mainPanel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(firstButtonMain);
        }

        // handle the case for cancelling or backing out of menus
        if (cancelAction.triggered)
        {
            if (optionsPanel.activeSelf)
                OnBackToMainFromOptionsButtonPressed();
            else if (playPanel.activeSelf)
                OnBackToMainFromPlayButtonPressed();
            else if (generalPanel.activeSelf)
                OnBackButtonInOptionsPressed(generalPanel);
            else if (soundPanel.activeSelf)
                OnBackButtonInOptionsPressed(soundPanel);
            else if (controlsPanel.activeSelf)
                OnBackButtonInOptionsPressed(controlsPanel);
        }

        // handle the case where if a player clicks on/off screen and can no longer use controller
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            if (reselectAction.triggered)
                HandleControllerReselect();
        }

        // handle changing the controls overlay based on the last input from the player
        // if (reselectAction.triggered)
    }

    private void HandleControllerReselect()
    {
        if (titlePanel.activeSelf)
            return;

        if (optionsPanel.activeSelf)
            EventSystem.current.SetSelectedGameObject(firstButtonOptions);
        else if (soundPanel.activeSelf)
            EventSystem.current.SetSelectedGameObject(firstButtonSound);
        else if (generalPanel.activeSelf)
            EventSystem.current.SetSelectedGameObject(firstButtonGeneral);
        else if (controlsPanel.activeSelf)
            EventSystem.current.SetSelectedGameObject(firstButtonControls);
        else if (mainPanel.activeSelf)
            EventSystem.current.SetSelectedGameObject(firstButtonMain);
        else if (playPanel.activeSelf)
            EventSystem.current.SetSelectedGameObject(firstButtonPlay);
    }

    private void HandleLoadGameAndContinueButtons()
    {
        loadGameButton.interactable = true;
        continueGameButton.interactable = true;
    }

    #endregion

    #region OnButtonClicks

    public void StartGame()
    {
        _=SaveSystem.LoadLevel();
    }

    public void QuitGame()
    {
        if (blocked) return;
        
        // if we are running in a standalone build of the game
        #if UNITY_STANDALONE
            Application.Quit();
        #endif

        // if we are running in the editor
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    
    public void OnOptionsButtonPressed()
    {
        if (blocked) return;
        // set correct panels
        optionsPanel.SetActive(true);
        mainPanel.SetActive(false);
        controlsOverlayPanel.SetActive(true);

        // set starting button for controller and keys input
        EventSystem.current.SetSelectedGameObject(firstButtonOptions);

        // switch the current camera view
        StartCoroutine(SwitchToNewCamera(mainCamera, optionsCamera));
    }
    
    public void OnSoundButtonPressed()
    {
        if (blocked) return;
        // set correct panels
        soundPanel.SetActive(true);
        optionsPanel.SetActive(false);

        // set starting button for controller and keys input
        EventSystem.current.SetSelectedGameObject(firstButtonSound);
    }

    public void OnControlsButtonPressed()
    {
        if (blocked) return;
        // set correct panels
        controlsPanel.SetActive(true);
        optionsPanel.SetActive(false);

        // set starting button for controller and keys input
        EventSystem.current.SetSelectedGameObject(firstButtonControls);
    }

    public void OnGeneralButtonPressed()
    {
        if (blocked) return;
        // set correct panels
        generalPanel.SetActive(true);
        optionsPanel.SetActive(false);

        // set starting button for controller and keys input
        EventSystem.current.SetSelectedGameObject(firstButtonGeneral);
    }

    public void OnPlayGameButtonPressed()
    {
        if (blocked) return;
        // set correct panels
        playPanel.SetActive(true);
        mainPanel.SetActive(false);
        controlsOverlayPanel.SetActive(true);

        // set starting button for controller and keys input
        EventSystem.current.SetSelectedGameObject(firstButtonPlay);

        // switch the current camera view
        StartCoroutine(SwitchToNewCamera(mainCamera, playGameCamera));
    }

    public void OnBackToMainFromPlayButtonPressed()
    {
        // set correct panels
        mainPanel.SetActive(true);
        playPanel.SetActive(false);
        controlsOverlayPanel.SetActive(false);

        // set starting button for controller and keys input
        EventSystem.current.SetSelectedGameObject(firstButtonMain);

        // switch the current camera view
        StartCoroutine(SwitchToNewCamera(playGameCamera, mainCamera));
    }

    public void OnBackToMainFromOptionsButtonPressed()
    {
        // set correct panels
        mainPanel.SetActive(true);
        optionsPanel.SetActive(false);
        controlsOverlayPanel.SetActive(false);

        // set starting button for controller and keys input
        EventSystem.current.SetSelectedGameObject(firstButtonMain);

        // switch the current camera view
        StartCoroutine(SwitchToNewCamera(optionsCamera, mainCamera));
    }

    public void OnBackButtonInOptionsPressed(GameObject panelToClose)
    {
        // set correct panels
        optionsPanel.SetActive(true);
        panelToClose.SetActive(false);

        // set starting button for controller and keys input
        EventSystem.current.SetSelectedGameObject(firstButtonOptions);
    }

    #endregion

    #region Camera + Fade

    private IEnumerator FadeOutCoroutine(GameObject panelToDisable)
    {
        CanvasGroup canvasGroup = panelToDisable.GetComponent<CanvasGroup>();
        float startAlpha = canvasGroup.alpha;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, timer / fadeDuration);
            yield return null;
        }

        panelToDisable.SetActive(false);
        canvasGroup.alpha = startAlpha;
        isPastTitleScreen = true;
    }

    private IEnumerator SwitchToNewCamera(CinemachineCamera oldCamera, CinemachineCamera newCamera)
    {
        oldCamera.gameObject.SetActive(false);
        newCamera.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(blendTime);
    }

    #endregion

    # region Options Buttons

    private void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
        PlayerPrefs.SetInt("Fullscreen", isFullScreen ? 1 : 0);
    }

    // public void ApplyGeneralOptions()
    // {
    //     // bool fullscreen = fullscreenToggle.desiredFullscreenState;
    //     SetFullScreen(fullscreen);
    //
    //     PlayerPrefs.SetInt("Fullscreen", fullscreen ? 1 : 0);
    //     PlayerPrefs.Save();
    // }

    private void SetMasterVolume(float volume)
    {
        AudioManager.Instance.AudioVolumeManager.SetVolume(AudioVolumeManager.AudioGroups.Master, volume);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    private void SetSFXVolume(float volume)
    {
        AudioManager.Instance.AudioVolumeManager.SetVolume(AudioVolumeManager.AudioGroups.SFX, volume);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    private void SetMusicVolume(float volume)
    {
        AudioManager.Instance.AudioVolumeManager.SetVolume(AudioVolumeManager.AudioGroups.Music, volume);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }
    
    private void SetDialogueVolume(float volume)
    {
        AudioManager.Instance.AudioVolumeManager.SetVolume(AudioVolumeManager.AudioGroups.Dialogue, volume);
        PlayerPrefs.SetFloat("DialogueVolume", volume);
    }
    
    private void SetAmbienceVolume(float volume)
    {
        AudioManager.Instance.AudioVolumeManager.SetVolume(AudioVolumeManager.AudioGroups.Ambience, volume);
        PlayerPrefs.SetFloat("AmbienceVolume", volume);
    }

    private void SetVolumeFromSlider(float value, string playerPrefKey, AudioVolumeManager.AudioGroups audioGroup)
    {
        float sliderValue = value; // Use slider.value (0-1)

        // Convert linear slider value (0-1) to logarithmic dB value (-80 to 0)
        float dbValue = Mathf.Lerp(-80f, 0f, Mathf.Pow(sliderValue, 0.5f));

        // Store the dB value in PlayerPrefs (good practice)
        PlayerPrefs.SetFloat(playerPrefKey, dbValue);

        // Set the actual volume.
        AudioManager.Instance.AudioVolumeManager.SetVolume(audioGroup, dbValue);
    }
    
    public void ApplySoundOptions()
    {
        SetVolumeFromSlider(masterVolumeSlider.fillAmount, "MasterVolume", AudioVolumeManager.AudioGroups.Master);
        SetVolumeFromSlider(sfxVolumeSlider.fillAmount, "SfxVolume", AudioVolumeManager.AudioGroups.SFX);
        SetVolumeFromSlider(musicVolumeSlider.fillAmount, "MusicVolume", AudioVolumeManager.AudioGroups.Music);
        SetVolumeFromSlider(ambienceVolumeSlider.fillAmount, "AmbienceVolume", AudioVolumeManager.AudioGroups.Ambience);
        SetVolumeFromSlider(dialogueVolumeSlider.fillAmount, "DialogueVolume", AudioVolumeManager.AudioGroups.Dialogue);
        // SetVolumeFromSlider(musicVolumeSlider.fillAmount, "MusicVolume", AudioVolumeManager.AudioGroups.Music);

        PlayerPrefs.Save();
        
        // float volume = PlayerPrefs.GetFloat("MasterVolume");
        // volume = Mathf.Lerp(-80f, 0f, masterVolumeSlider.fillAmount);
        // SetMasterVolume(volume);
        //
        // volume = PlayerPrefs.GetFloat("SfxVolume");
        // volume = Mathf.Lerp(-80f, 0f, sfxVolumeSlider.fillAmount);
        // SetSFXVolume(volume);
        //
        // volume = PlayerPrefs.GetFloat("MusicVolume");
        // volume = Mathf.Lerp(-80f, 0f, musicVolumeSlider.fillAmount);
        // SetMusicVolume(volume);
        //
        // PlayerPrefs.Save();
    }

    #endregion

    #region Saving/Loading

    public void NewGame()
    {
        StartCoroutine(WaitForCameraSwitcherNewGame());
    }

    private IEnumerator WaitForCameraSwitcherNewGame()
    {
        yield return new WaitForSeconds(0.5f);
        
        ResetSaveFile();
        LoadSaveFile();
    }
    
    private IEnumerator WaitForCameraSwitcherLoadGame()
    {
        yield return new WaitForSeconds(2f);
        
        _ = SaveSystem.LoadLevel();
    }

    public void LoadSaveFile()
    {
        StartCoroutine(WaitForCameraSwitcherLoadGame());
    }

    public void ResetSaveFile()
    {
        SaveSystem.ResetSave();
    }
    
    public void SaveSaveFile()
    {
        SaveSystem.Save();
    }

    #endregion

    void BlockInput(InputAction.CallbackContext ctx)
    {
        StartCoroutine(BlockInput());
    }

    IEnumerator BlockInput()
    {
        yield return null;
        blocked = true;
        foreach (var button in allButtons)
        {
            button.enabled = false;
        }

        yield return new WaitForSeconds(1.5f);

        blocked = false;
        foreach (var button in allButtons)
        {
            button.enabled = true;
        }
    }

    private void Start()
    {
        allButtons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
    }
}