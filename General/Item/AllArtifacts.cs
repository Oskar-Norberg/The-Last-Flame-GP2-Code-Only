using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEditor;
using General.Item;


[CreateAssetMenu(fileName = "ArtifactsCollection", menuName = "Scriptable Objects/Artifacts", order = 1)]
public class AllArtifacts : ScriptableObject
{
    //public Dictionary<int, int> artifactIndexPair = new Dictionary<int, int>();
    public List<string> artifacts = new List<string>();


    public int total;

#if UNITY_EDITOR
    [ContextMenu("Get all artifacts")]
    public void GetAllArtifacts()
    {
        artifacts = new List<string>();

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            var scene = EditorBuildSettings.scenes[i];
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scene.path);


            var all = MonoBehaviour.FindObjectsByType<Artifact>(FindObjectsSortMode.InstanceID);
            bool[] check = new bool[all.Length];
            for (int j = 0; j < all.Length; j++)
            {
                EditorUtility.SetDirty(all[j]);

                all[j].index = artifacts.Count;
                artifacts.Add(i + "_" + artifacts.Count);
            }



            UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
        }

        total = artifacts.Count;
    }
#endif
}
