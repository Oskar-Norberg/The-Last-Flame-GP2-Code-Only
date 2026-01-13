using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.InputSystem;

public class ImageFillVolumeControl : MonoBehaviour, IPointerDownHandler, IDragHandler, ISelectHandler, IDeselectHandler
{
    [Header("UI Elements")]
    [SerializeField] private Image volumeFillImage;
    [SerializeField] private TextMeshProUGUI volumeValueText;
    [SerializeField] private Color textColorOnSelected;
    
    [Header("Input Actions")]
    [SerializeField] private InputActionAsset inputActionAsset;
    private InputAction navigateAction;
    private Vector2 navigateValue;
    
    [Header("Input Settings")]
    [SerializeField] private float initialInputDelay = 0.2f;
    [SerializeField] private float acceleratedInputDelay = 0.05f;
    [SerializeField] private float holdTimeThreshold = 0.5f;

    private float lastInputTime = -1f;
    private float timeButtonPressed = -1f;
    private float inputDelay;
    
    private RectTransform fillRectTransform;
    private float volume = 1f;
    private bool isSelected = false;
    private Camera mainCam;
    [SerializeField] private VolumeType volumeType;

    enum VolumeType
    {
        Master,
        Music,
        SFX,
        Dialogue,
        Ambience
    }
    
    private void Awake()
    {
        mainCam = Camera.main;
        fillRectTransform = volumeFillImage.rectTransform;
    }

    void Start()
    {
        LoadVolume();
    }
    
    private void OnEnable()
    {
        UpdateVisuals();

        InputActionMap uiActionMap = inputActionAsset.FindActionMap("UI");
        navigateAction = uiActionMap.FindAction("Navigate");
        navigateAction.Enable();
    }

    private void OnDisable()
    {
        navigateAction.Disable();
    }

    public void OnPointerDown(PointerEventData eventData) => HandleInput(eventData);

    public void OnDrag(PointerEventData eventData) => HandleInput(eventData);

    private void HandleInput(PointerEventData eventData)
    {
        Vector2 localMousePosition;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(fillRectTransform, eventData.position, mainCam, out localMousePosition))
        {
            float fillPercentage = Mathf.Clamp01(localMousePosition.x / fillRectTransform.rect.width);
            volume = fillPercentage;
            UpdateVisuals();
        }

        switch (volumeType)
        {
            case VolumeType.Master:
                SettingsManager.Instance.SetMasterVolume(volume);
                break;
            case VolumeType.Music:
                SettingsManager.Instance.SetMusicVolume(volume);
                break;
            case VolumeType.SFX:
                SettingsManager.Instance.SetSFXVolume(volume);
                break;
            case VolumeType.Dialogue:
                SettingsManager.Instance.SetDialogueVolume(volume);
                break;
            case VolumeType.Ambience:
                SettingsManager.Instance.SetAmbienceVolume(volume);
                break;
        }
    }

    private void LoadVolume()
    {
        // Get the correct preview value from the settings
        float loadedVolume = 0;
        switch (volumeType)
        {
            case VolumeType.Master:
                loadedVolume = SettingsManager.Instance.gameSettings.volumeSettings.masterVolumePreview;
                break;
            case VolumeType.Music:
                loadedVolume = SettingsManager.Instance.gameSettings.volumeSettings.musicVolumePreview;
                break;
            case VolumeType.SFX:
                loadedVolume = SettingsManager.Instance.gameSettings.volumeSettings.sfxVolumePreview;
                break;
            case VolumeType.Dialogue:
                loadedVolume = SettingsManager.Instance.gameSettings.volumeSettings.dialogueVolumePreview;
                break;
            case VolumeType.Ambience:
                loadedVolume = SettingsManager.Instance.gameSettings.volumeSettings.ambienceVolumePreview;
                break;
        }

        volumeFillImage.fillAmount = loadedVolume; // Set the fill image amount

        int displayedVolume = Mathf.RoundToInt(loadedVolume * 100);
        volumeValueText.text = displayedVolume.ToString(); // Update the text
    }
    
    private void UpdateVisuals()
    {
        volumeFillImage.fillAmount = volume;
        volumeValueText.text = Mathf.RoundToInt(volume * 100).ToString();
    }

    public void OnSelect(BaseEventData eventData)
    {
        isSelected = true;
        volumeValueText.color = textColorOnSelected;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;
        volumeValueText.color = Color.white;
    }

    public void ResetSoundSlider()
    {
        isSelected = false;
        volumeValueText.color = Color.white;
    }

    private void Update()
    {
        if (isSelected)
        {
            navigateValue = navigateAction.ReadValue<Vector2>();

            if (Mathf.Abs(navigateValue.x) > 0)
            {
                inputDelay = Time.time - timeButtonPressed > holdTimeThreshold ? acceleratedInputDelay : initialInputDelay;

                if (Time.time - lastInputTime > inputDelay)
                {
                    float changeAmount = navigateValue.x > 0 ? 0.01f : -0.01f;
                    volume = Mathf.Clamp01(volume + changeAmount);
                    UpdateVisuals();
                    lastInputTime = Time.time;
                    timeButtonPressed = timeButtonPressed == -1f ? Time.time : timeButtonPressed;
                }
            }
            else
            {
                timeButtonPressed = -1f;
            }
        }
    }
}