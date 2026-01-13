using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CodexNavigation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Pages")]
    [SerializeField] private GameObject artifactsPage;
    [SerializeField] private GameObject memoriesPage;

    [Header("Artifacts Info")]
    [SerializeField] private GameObject[] artifacts;
    [SerializeField] private GameObject[] indicators;
    [SerializeField] private SubtitleSO[] artifactTexts;
    [SerializeField] private TextMeshProUGUI currentArtifactText;

    [Header("Buttons")]
    [SerializeField] private GameObject firstButtonArtifacts;
    [SerializeField] private GameObject firstButtonMemories;
    [SerializeField] private GameObject titleButtonArtifacts;
    [SerializeField] private GameObject titleButtonMemories;

    [Header("Inputs")]
    [SerializeField] private InputActionAsset inputActionAsset;
    private InputAction cancelAction;
    private InputAction submitAction;
    private InputAction reselectAction;
    private InputAction tabLeft, tabRight;


    private int currentSelectedIndex = -1;

    private void Start() {
        UnlockCursor();
    }

    public void OnEnable() {
        EventSystem.current.SetSelectedGameObject(firstButtonArtifacts);

        InputActionMap uiActionMap = inputActionAsset.FindActionMap("UI");

        cancelAction = uiActionMap.FindAction("Cancel");
        cancelAction.Enable();

        submitAction = uiActionMap.FindAction("Submit");
        submitAction.Enable(); 

        reselectAction = uiActionMap.FindAction("Navigate");
        reselectAction.Enable(); 

        tabRight = uiActionMap.FindAction("TabRight");
        tabRight.Enable();

        tabLeft = uiActionMap.FindAction("TabLeft");
        tabLeft.Enable();
        
        OnArtifactsButtonClicked();
    }

    private void OnDisable() {
        cancelAction.Disable();
        submitAction.Disable();
        reselectAction.Disable();
        tabRight.Disable();
        tabLeft.Disable();
    }

    private void Update() {
        // handle reselection case
        if (reselectAction.triggered && EventSystem.current.currentSelectedGameObject == null)
        {
            if (artifactsPage.activeSelf)
                EventSystem.current.SetSelectedGameObject(firstButtonArtifacts);
            else if (memoriesPage.activeSelf)
                EventSystem.current.SetSelectedGameObject(firstButtonMemories);
        }
        
        // allow for tabbing between pages when on controller
        if (tabLeft.triggered && artifactsPage.activeSelf)
            OnMemoriesButtonClicked();
        else if (tabRight.triggered && memoriesPage.activeSelf)
            OnArtifactsButtonClicked();

        if (artifactsPage.activeSelf)
        {
            Color textColor1 = titleButtonArtifacts.GetComponent<TextMeshProUGUI>().color;
            Color textColor2 = titleButtonMemories.GetComponent<TextMeshProUGUI>().color;
            titleButtonArtifacts.GetComponent<TextMeshProUGUI>().color = new Color(textColor1.r, textColor1.g, textColor1.b, 1f);
            titleButtonMemories.GetComponent<TextMeshProUGUI>().color = new Color(textColor2.r, textColor2.g, textColor2.b, 0.1f);
        }
        if (memoriesPage.activeSelf)
        {
            Color textColor1 = titleButtonArtifacts.GetComponent<TextMeshProUGUI>().color;
            Color textColor2 = titleButtonMemories.GetComponent<TextMeshProUGUI>().color;
            titleButtonArtifacts.GetComponent<TextMeshProUGUI>().color = new Color(textColor1.r, textColor1.g, textColor1.b, 0.1f);
            titleButtonMemories.GetComponent<TextMeshProUGUI>().color = new Color(textColor2.r, textColor2.g, textColor2.b, 1f);
        }
            // () = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, 0.1f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        for (int i = 0; i < artifacts.Length; i++)
        {
            if (eventData.pointerCurrentRaycast.gameObject == artifacts[i])
            {
                UpdateCurrentArtifactSelection(i);
                break;
            }
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        UpdateCurrentArtifactSelection(-1);
    }
    
    public void UpdateCurrentArtifactSelection(int index)
    {
        foreach (GameObject indicator in indicators)
            indicator.SetActive(false);
    
        currentSelectedIndex = index;
        
        // print(index);
    
        if (index >= 0 && index < artifacts.Length)
        {
            indicators[index].SetActive(true);

            if (index >= 4 && index <= 9)
            {
                if (artifacts[index].transform.parent.GetComponent<Image>().color == Color.white)
                    currentArtifactText.text = "Artifact " + (index + 1) + "\n" + artifactTexts[index].text;
                else
                    currentArtifactText.text = "Missing Artifact " + (index + 1) +  ": Not Collected Yet";
            }
            else
            {
                if (artifacts[index].GetComponent<Image>().color == Color.white)
                    currentArtifactText.text = "Artifact " + (index + 1) + "\n" + artifactTexts[index].text;
                else
                    currentArtifactText.text = "Missing Artifact " + (index + 1) +  ": Not Collected Yet";
            }
        }
        else
        {
            currentArtifactText.text = "";
        }
    }
    
    public void AddNewArtifact(int index)
    {
        // if one of the indexes for the broken pieces:
        if (index >= 4 && index <= 9)
        {
            artifacts[index].transform.parent.GetComponent<Image>().color = Color.white;
        }
        else
        {
            // change the display to show the artifact
            artifacts[index].GetComponent<Image>().color = Color.white;
        }
        
        // check if the newly added artifact is the currently selected one.
        // if (currentSelectedIndex == index)
        // {
        //     UpdateCurrentArtifactSelection(index);
        // }
    }
    
    public void ResetArtifacts()
    {
        // change the display to show the artifact
        for (int i = 0; i < artifacts.Length; i++)
        {
            if (i >= 4 && i <= 9)
            {
                artifacts[i].transform.parent.GetComponent<Image>().color = Color.black;
            }
            else
            {
                // change the display to show the artifact
                artifacts[i].GetComponent<Image>().color = Color.black;
            }
        }
        // artifacts[i].GetComponent<Image>().color = Color.black;
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked; // Locks cursor to center of game window
        Cursor.visible = false; // Hides the cursor (optional)
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None; // Unlocks the cursor
        Cursor.visible = true; // Makes the cursor visible again
    }

    public void OnArtifactsButtonClicked()
    {
        memoriesPage.SetActive(false);
        artifactsPage.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstButtonArtifacts);
    }

    public void OnMemoriesButtonClicked()
    {
        artifactsPage.SetActive(false);
        memoriesPage.SetActive(true);
        // EventSystem.current.SetSelectedGameObject(firstButtonMemories);
    }
}

