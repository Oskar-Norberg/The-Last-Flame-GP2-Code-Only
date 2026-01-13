using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.Audio;

public class SettingsManager : PersistentSingleton<SettingsManager>
{
    public static SettingsManager Instance { get; private set; }
    public GameSettings gameSettings;
    [SerializeField] private AudioMixer audioMixer;
    private string savePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        gameSettings = new GameSettings();
        savePath = Application.persistentDataPath + "/settings.json";
        print(savePath);
    }

    void Start()
    {
        // this doesn't work in Awake, has to be in Start!
        LoadSettings();
    }

    public void SaveSettings()
    {
        string json = JsonConvert.SerializeObject(gameSettings, Formatting.Indented);
        File.WriteAllText(savePath, json);
    }

    public void LoadSettings()
    {
        if (File.Exists(savePath))
        {
            try
            {
                string json = File.ReadAllText(savePath);
                gameSettings = JsonConvert.DeserializeObject<GameSettings>(json);

                ApplyAllGameSettings();
            }
            catch (JsonException ex)
            {
                Debug.LogError("Error loading settings: " + ex.Message);
            }
        }
    }

    public void SetHUDEnabled(bool enabled)
    {
        gameSettings.generalSettings.hudEnabledPreview = enabled;
    }

    public void SetFullscreen(bool enabled)
    {
        gameSettings.generalSettings.fullscreenPreview = enabled;
    }
    
    public void SetSubtitles(bool enabled)
    {
        gameSettings.generalSettings.subtitlesPreview = enabled;
    }
    
    public void SetSprintToggle(bool enabled)
    {
        gameSettings.generalSettings.sprintIsTogglePreview = enabled;
    }

    public void SetMasterVolume(float volume)
    {
        if (volume == 0)
            gameSettings.volumeSettings.masterVolumePreview = 0.004f;
        else
            gameSettings.volumeSettings.masterVolumePreview = volume;
    }

    public void SetMusicVolume(float volume)
    {
        if (volume == 0)
            gameSettings.volumeSettings.musicVolumePreview = 0.004f;
        else
            gameSettings.volumeSettings.musicVolumePreview = volume;
    }

    public void SetSFXVolume(float volume)
    {
        if (volume == 0)
            gameSettings.volumeSettings.sfxVolumePreview = 0.004f;
        else
            gameSettings.volumeSettings.sfxVolumePreview = volume;
    }
    
    public void SetDialogueVolume(float volume)
    {
        if (volume == 0)
            gameSettings.volumeSettings.dialogueVolumePreview = 0.004f;
        else
            gameSettings.volumeSettings.dialogueVolumePreview = volume;
    }
    
    public void SetAmbienceVolume(float volume)
    {
        if (volume == 0)
            gameSettings.volumeSettings.ambienceVolumePreview = 0.004f;
        else
            gameSettings.volumeSettings.ambienceVolumePreview = volume;
    }

    public void ApplyVolumeSettings()
    {
        gameSettings.volumeSettings.masterVolume = gameSettings.volumeSettings.masterVolumePreview;
        gameSettings.volumeSettings.musicVolume = gameSettings.volumeSettings.musicVolumePreview;
        gameSettings.volumeSettings.sfxVolume = gameSettings.volumeSettings.sfxVolumePreview;
        gameSettings.volumeSettings.dialogueVolume = gameSettings.volumeSettings.dialogueVolumePreview;
        gameSettings.volumeSettings.ambienceVolume = gameSettings.volumeSettings.ambienceVolumePreview;

        audioMixer.SetFloat("MasterVolume", Mathf.Log10(gameSettings.volumeSettings.masterVolume) * 20);
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(gameSettings.volumeSettings.musicVolume) * 20);
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(gameSettings.volumeSettings.sfxVolume) * 20);
        audioMixer.SetFloat("DialogueVolume", Mathf.Log10(gameSettings.volumeSettings.dialogueVolume) * 20);
        audioMixer.SetFloat("AmbienceVolume", Mathf.Log10(gameSettings.volumeSettings.ambienceVolume) * 20);

        SaveSettings();
    }

    public void ApplyGeneralSettings()
    {
        // fullscreen
        gameSettings.generalSettings.fullscreen = gameSettings.generalSettings.fullscreenPreview;
        if (gameSettings.generalSettings.fullscreen)
            Screen.fullScreen = true;
        else
            Screen.fullScreen = false;

        // toggle HUD
        gameSettings.generalSettings.hudEnabled = gameSettings.generalSettings.hudEnabledPreview;
            
        // toggle subtitles
        gameSettings.generalSettings.subtitles = gameSettings.generalSettings.subtitlesPreview;
        
        // toggle sprint
        gameSettings.generalSettings.sprintIsToggle = gameSettings.generalSettings.sprintIsTogglePreview;
        
        SaveSettings();
    }

    private void ApplyAllGameSettings()
    {
        ApplyVolumeSettings();
        ApplyGeneralSettings();
    }
}