using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEditor;


[CreateAssetMenu(fileName = "AllCheckpoints", menuName = "Scriptable Objects/Checkpoints", order = 1)]
public class AllCheckpoints : ScriptableObject
{
    //public Dictionary<int, int> checkpointIndexPair = new Dictionary<int, int>();
    public List<string> checkpoints = new List<string>();


    public int total;

    #if UNITY_EDITOR
    [ContextMenu("Get all checkpoints")]
    public void GetAllCheckpoints()
    {
        checkpoints = new List<string>();

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            var scene = EditorBuildSettings.scenes[i];
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scene.path);


            var all = MonoBehaviour.FindObjectsByType<CheckPoint>(FindObjectsSortMode.InstanceID);
            bool[] check = new bool[all.Length];
            for (int j = 0; j < all.Length; j++)
            {
                EditorUtility.SetDirty(all[j]);

                all[j].saveIndex = checkpoints.Count;
                checkpoints.Add(i + "_" + checkpoints.Count);
            }


            
            UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
        }

        total = checkpoints.Count;
    }
    #endif
}
