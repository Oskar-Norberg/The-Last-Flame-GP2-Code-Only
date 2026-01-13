using TMPro;
using UnityEngine;

public class MenuTabs : MonoBehaviour
{
    [SerializeField] private GameObject[] tabs;
    // public Color hoverColor = new Color(238f, 183f, 103f, 1f);

    public void TurnOnTabs(int tabIndex)
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            tabs[i].SetActive(false);
        }
        tabs[tabIndex].SetActive(true);
        
        // tabs[tabIndex].transform.GetComponentInParent<TextMeshProUGUI>().color = Color.white;
    }
}
