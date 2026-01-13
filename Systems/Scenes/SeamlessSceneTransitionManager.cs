using UnityEngine;

public class SeamlessSceneTransitionManager : MonoBehaviour
{
    public int nextScene;
    public int prevScene;

    [Space(10)]
    public SeamlessSceneTransition nextLoad;
    public SeamlessSceneTransition nextUnload;

    public SeamlessSceneTransition prevLoad;
    public SeamlessSceneTransition prevUnload;

    private void OnValidate()
    {
        nextLoad.sceneInBuild = nextScene;
        nextUnload.sceneInBuild = nextScene;

        prevLoad.sceneInBuild = prevScene;
        prevUnload.sceneInBuild = prevScene;
    }

    /*
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        SeamlessSceneTransitionManager[] all = FindObjectsByType<SeamlessSceneTransitionManager>(FindObjectsSortMode.None);
        foreach (var t in all)
        {
            if (t != this && t.nextScene == nextScene && t.prevScene == prevScene)
            {
                Destroy(gameObject);
                break;
            }
        }
    }
    */
}