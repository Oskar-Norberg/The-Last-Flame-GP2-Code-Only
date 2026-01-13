using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Timeline;

[CreateAssetMenu(fileName = "CutsceneCollection", menuName = "Cutscene/Collection")]
public class AllDialogs : ScriptableObject
{
    private const string CutscenePath =  "Timeline";
    
    public TimelineAsset[] cutscenes;

    // private void OnValidate()
    // {
    //     subtitles = subtitles.ToArray();
    // }

    #if (UNITY_EDITOR) 
    [ContextMenu("Load Cutscenes")]
    public void LoadCutscenes()
    {
        List<TimelineAsset> playables = new();
        
        List<string> paths = new();

        string path = Path.Combine(Application.dataPath, CutscenePath);
        
        string[] dirs = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);


        // Convert from full-system paths to unity database Asset relative paths
        for (int i = 0; i < dirs.Length; i++)
        {
            int start = dirs[i].LastIndexOf("Assets");
            
            if (start != -1)
            {
                dirs[i] = dirs[i].Substring(start);
            }
            else
            {
                dirs[i] = string.Empty;
            }
        }


        Debug.Log(dirs.Length);

        foreach (string guid in AssetDatabase.FindAssets("t:TimelineAsset", dirs))
        {
            playables.Add( AssetDatabase.LoadAssetAtPath<TimelineAsset>(AssetDatabase.GUIDToAssetPath(guid)));
        }
        
        
        // RecursiveLoad(SubtitlePath, ref subtitleSOs);
        // foreach (string guid in AssetDatabase.FindAssets("t:SubtitleSO", new string[] { "Assets/Enemies/Waves" }))
        // {
        //     subtitleSOs.Add( AssetDatabase.LoadAssetAtPath<SubtitleSO>(AssetDatabase.GUIDToAssetPath(guid)));
        // }
        //
        cutscenes = playables.ToArray();
    }
    #endif
}
