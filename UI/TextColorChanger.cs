using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextColorChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Color hoverColor = new Color(238f, 183f, 103f, 1f); 
    public Color clickColor = new Color(238f, 183f, 103f, 1f);
    public Color defaultColor = Color.white;

    private TextMeshProUGUI textMeshPro;

    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        textMeshPro.color = defaultColor; 
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        textMeshPro.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        textMeshPro.color = defaultColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        textMeshPro.color = clickColor;
    }
}