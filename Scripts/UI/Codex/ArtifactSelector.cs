using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArtifactSelector : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private bool isSelected;
    [SerializeField] private GameObject artifactSelector;
    [SerializeField] private TextMeshProUGUI currentArtifactText;
    [SerializeField] private SubtitleSO artifactTextString;
    [SerializeField] private int index;
    [SerializeField] private CodexNavigation codexNavigation;

    public void OnSelect(BaseEventData eventData)
    {
        isSelected = true;
        artifactSelector.SetActive(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;
        artifactSelector.SetActive(false);
    }

    private void ResetSelection()
    {
        isSelected = false;
        artifactSelector.SetActive(false);
    }

    private void UpdateVisuals()
    {
        // if (gameObject.GetComponent<Image>().color == Color.white)
        //     currentArtifactText.text = artifactTextString.text;
        // else
        //     currentArtifactText.text = "???? Missing: Artifact Not Collected Yet ????";
        codexNavigation.UpdateCurrentArtifactSelection(index);
    }

    void Update()
    {
        if (isSelected) 
            UpdateVisuals();
    }
}
