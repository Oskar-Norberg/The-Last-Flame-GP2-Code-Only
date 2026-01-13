using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GlobalButtonClicked : MonoBehaviour
{
    [Header("Audio Setup")]
    [SerializeField] private AudioSource clickAudioSource;
    [SerializeField] private AudioClip clickAudioClip;

    private void Start()
    {
        // Find all buttons (including inactive ones) with no particular sorting
        Button[] allButtons = Object.FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (Button btn in allButtons)
        {
            EventTrigger eventTrigger = btn.gameObject.GetComponent<EventTrigger>();
            if (eventTrigger == null)
                eventTrigger = btn.gameObject.AddComponent<EventTrigger>();

            // Create a PointerEnter Entry
            EventTrigger.Entry hoverEntry = new EventTrigger.Entry();
            hoverEntry.eventID = EventTriggerType.PointerClick;
            hoverEntry.callback.AddListener((data) => { OnButtonClicked(); });

            eventTrigger.triggers.Add(hoverEntry);
        }
    }

    private void OnButtonClicked()
    {
        if (clickAudioSource != null && clickAudioClip != null)
        {
            clickAudioSource.PlayOneShot(clickAudioClip);
        }
    }
}