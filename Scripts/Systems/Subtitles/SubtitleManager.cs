using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubtitleManager : MonoBehaviour
{
    [SerializeField] private Image panel;
    [SerializeField] private TextMeshProUGUI textMesh;

    [Header("Use This max panel instead of setting the alpha channel on the color.")]
    [SerializeField] [Range(0.0f, 1.0f)] private float maxPanelOpacity = 0.4f;
    [SerializeField] private float paddingX, paddingY;
    [SerializeField] private Color aureliaColor;
    [SerializeField] private Color narratorColor;
    // [SerializeField] private Color speakerNameColor;
    
    
    private void Awake(){
        // TODO: This isn't a hack per se, but it is quite unseemly - Oskar
        
        // Hide panel on startup
        Color panelColor = panel.color;
        panelColor.a = 0.0f;
        panel.color = panelColor;
        
        // Hide text on startup
        Color textColor = textMesh.color;
        textColor.a = 0.0f;
        textMesh.color = textColor;
    }

    public void SetText(string text, Color textColor, float weight)
    {
        textColor.a = Mathf.Lerp(0.0f, 1.0f, weight);
        textMesh.color = textColor;
        
        Color panelColor = panel.color;
        panelColor.a = Mathf.Lerp(0.0f, maxPanelOpacity, weight);
        panel.color = panelColor;
        
        // apply the speaker name formatting if necessary
        if (ShouldApplyStyling(text))
            text = FormatText(text);
        
        textMesh.text = text;
    }

    public void UpdateTextBoxSize()
    {
        Vector2 textSize = textMesh.GetPreferredValues();
        RectTransform imageRect = GetComponent<Image>().rectTransform;
        imageRect.sizeDelta = textSize + new Vector2(paddingX, paddingY);
    }
    
    public string FormatText(string text)
    {
        string[] words = text.Split(' ');

        if (words.Length > 0)
        {
            string firstWord = words[0];
            string coloredFirstWord;

            if (firstWord.Equals("Aurelia", StringComparison.OrdinalIgnoreCase))
            {
                coloredFirstWord = $"<b><color=#{ColorUtility.ToHtmlStringRGB(aureliaColor)}>{firstWord}</color></b>";
            }
            else if (firstWord.Equals("Narrator", StringComparison.OrdinalIgnoreCase))
            {
                coloredFirstWord = $"<i><b><color=#{ColorUtility.ToHtmlStringRGB(narratorColor)}>{firstWord}</color></b></i>";
            }
            else
            {
                coloredFirstWord = firstWord; 
            }

            return coloredFirstWord + " " + string.Join(" ", words, 1, words.Length - 1);
        }

        return text;
    }
    
    private bool ShouldApplyStyling(string text)
    {
        string[] words = text.Split(' ');

        if (words.Length > 0)
        {
            string firstWord = words[0];
            return firstWord.Equals("Aurelia", StringComparison.OrdinalIgnoreCase) || 
                   firstWord.Equals("Narrator", StringComparison.OrdinalIgnoreCase);
        }

        return false; 
    }
}
