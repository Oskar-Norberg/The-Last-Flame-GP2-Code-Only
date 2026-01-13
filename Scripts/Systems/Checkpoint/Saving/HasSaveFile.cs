using UnityEngine;
using UnityEngine.UI;

public class HasSaveFile : MonoBehaviour
{
    Button button;
    void Start()
    {
        button = GetComponent<Button>();
        button.interactable = SaveSystem.HasSaveFile();
    }
}
