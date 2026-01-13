using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GlobalButtonHover : MonoBehaviour
{
    [Header("Audio Setup")]
    [SerializeField] private AudioSource hoverAudioSource;
    [SerializeField] private AudioClip hoverClip;

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
            hoverEntry.eventID = EventTriggerType.PointerEnter;
            hoverEntry.callback.AddListener((data) => { OnButtonHover(); });

            eventTrigger.triggers.Add(hoverEntry);
        }
    }

    private void OnButtonHover()
    {
        if (hoverAudioSource != null && hoverClip != null)
        {
            hoverAudioSource.PlayOneShot(hoverClip);
        }
    }
}