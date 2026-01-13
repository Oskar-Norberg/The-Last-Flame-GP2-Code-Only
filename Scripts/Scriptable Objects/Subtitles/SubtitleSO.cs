using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "Scriptable Objects/Dialogue")]
public class SubtitleSO : ScriptableObject
{
    public string text;
    public Color color = Color.white;
}