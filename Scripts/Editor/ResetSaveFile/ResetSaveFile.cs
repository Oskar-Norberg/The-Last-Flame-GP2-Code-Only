using UnityEditor;
using UnityEngine;

public static class ResetSaveFile
{
    [MenuItem("Tools/Reset Save File")]
    private static void ResetSave()
    {
        SaveSystem.ResetSave();
    }
}
