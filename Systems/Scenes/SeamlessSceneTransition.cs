using General.Player;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SeamlessSceneTransition : SceneTransition
{
    [Header("If checked it will load the scene, if unchecked it will unload it instead.")]
    public bool load;

    public static List<int> loadedScenes = new();

    private BoxCollider _col;

    private void Start()
    {
        if(loadedScenes.Count == 0) loadedScenes.Add(SceneManager.GetSceneAt(0).buildIndex);
    }

    private void OnValidate()
    {
        if(_col == null) _col = GetComponent<BoxCollider>();
    }

    protected override void OnEnter()
    {
        if (load) _= LoadScene();
        else UnloadScene();
    }

    public async Awaitable LoadScene()
    {
        if(!loadedScenes.Contains(sceneInBuild))
        {
            SingleInScene[] singles = FindObjectsByType<SingleInScene>(FindObjectsSortMode.None);

            foreach(var s in singles)
            {
                s.transform.SetParent(null, true);
                DontDestroyOnLoad(s.gameObject);
            }

            loadedScenes.Add(sceneInBuild);

            await SceneManager.LoadSceneAsync(sceneInBuild, LoadSceneMode.Additive);


            SingleInScene[] toRemove = FindObjectsByType<SingleInScene>(FindObjectsSortMode.None);

            toRemove = toRemove.Where(p => !singles.Contains(p)).ToArray();

            foreach(var r in toRemove)
            {
                r.gameObject.SetActive(false);
                Destroy(r);
            }
        }
    }
    public void UnloadScene()
    {
        bool unload = false;
        for(int i = 0; i < SceneManager.sceneCount; i++)
        {
            if(SceneManager.GetSceneAt(i).buildIndex == sceneInBuild)
            {
                unload = true;
                break;
            }
        }

        if (unload)
        {
            SceneManager.UnloadSceneAsync(sceneInBuild);
            loadedScenes.Remove(sceneInBuild);
        }
    }
    public void UnloadScene(ref int scene)
    {
        SceneManager.UnloadSceneAsync(scene);
        loadedScenes.Remove(scene);
        scene = -1;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = load ? new Color(.6f, 0, 0.6f, .35f) : new Color(1, 0, 0, .35f);
        Gizmos.DrawCube(transform.position, _col.size);
    }

}