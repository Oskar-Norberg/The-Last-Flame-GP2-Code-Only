using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class Thoughts : MonoBehaviour, IScrollHandler
{
    [SerializeField] private RectTransform thoughtsParent;
    [SerializeField] private GameObject thoughtTextPrefab;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float scrollSpeed = 5f;

    //testing
    [SerializeField] private SubtitleSO[] subtitleSOs;
    [SerializeField] private CodexLines[] codexLinesSO;
    private int i = 0;
    
    [Header("Inputs")]
    [SerializeField] private InputActionAsset inputActionAsset;
    private InputAction controllerScrollAction;

    private void Start()
    {
        DialogueLog.Instance.onDialogueLogUpdated += UpdateThoughts;
    }

    private void OnEnable()
    {
        // InputActionMap uiActionMap = inputActionAsset.FindActionMap("UI");
        // controllerScrollAction = uiActionMap.FindAction("ControllerScroll");
        // controllerScrollAction.Enable(); 
    }

    private void OnDisable()
    {
        // controllerScrollAction.Disable();
    }
    
    private void OnDestroy()
    {
        if (Application.isPlaying)
            DialogueLog.Instance.onDialogueLogUpdated -= UpdateThoughts;
    }

    public void OnScroll(PointerEventData eventData)
    {
        // Calculate the scroll amount based on the delta and your custom speed:
        float scrollAmount = eventData.scrollDelta.y * scrollSpeed * Time.unscaledDeltaTime;

        // Apply the scroll amount to the vertical normalized position:
        scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition + scrollAmount);
    }

    private void HandleControllerScrolling()
    {
        Vector2 scrollInput = controllerScrollAction.ReadValue<Vector2>();
        
        float scrollDelta = -scrollInput.y * scrollSpeed * Time.deltaTime; // For vertical scrolling (adjust for horizontal)

        // Adjust scroll position
        if (scrollRect.vertical)
        {
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition + scrollDelta);
        }
        else if (scrollRect.horizontal)
        {
            scrollRect.horizontalNormalizedPosition = Mathf.Clamp01(scrollRect.horizontalNormalizedPosition + scrollDelta);
        }
    }

    private void Update() {
        // HandleControllerScrolling();
        //
        //  if (Input.GetKeyDown(KeyCode.T))
        //  {
        //      string thoughtText = subtitleSOs[i].text;
        //      if (!string.IsNullOrEmpty(thoughtText))
        //      {
        //          CreateThought(thoughtText);
        //          LayoutRebuilder.ForceRebuildLayoutImmediate(thoughtsParent);
        //          scrollRect.verticalNormalizedPosition = 1f;
        //          i++;
        //      }
        //  }
    }

    private void UpdateThoughts(PlayableDirector playableDirector)
    {
        foreach (CodexLines codexLines in codexLinesSO)
        {
            if (codexLines.codexLineAsset == playableDirector.playableAsset)
                CreateThought(codexLines.codexLine);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(thoughtsParent);

        // scroll to top
        scrollRect.verticalNormalizedPosition = 1f;
    }

    private void CreateThought(string text)
    {
        var thoughtText = Instantiate(thoughtTextPrefab, thoughtsParent, false);
        thoughtText.GetComponent<TextMeshProUGUI>().text = text;

        // send it to the top!
        thoughtText.transform.SetAsFirstSibling();
    }
}