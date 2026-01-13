using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : Trigger
{
    public int sceneInBuild;
    public static bool sceneIsLoading = false;
    public static Material transitionVisual; 
    private const float _speed = 1;

    protected override void OnEnter()
    {
        _=TransitionScene(sceneInBuild);
    }

    //THESE SHOULD BE USED FOR SCENE TRANSITION CUZ THEY 
    //CAN HAVE VISUAL FUNCITONALITY ADDED TO THEM :3 - milo
    public static async Awaitable TransitionScene(int scene)
    {
        if (sceneIsLoading) return;

        sceneIsLoading = true;

        if(transitionVisual == null)
        {
            var g = Resources.Load<GameObject>("SceneTransition/TransitionVisual");
            var image = Instantiate(g).GetComponentInChildren<Image>();
            transitionVisual = image.material;
            DontDestroyOnLoad(image.transform.parent);
        }

        //wait and do visuals n shit
        await FadeInVisual();

        await SceneManager.LoadSceneAsync(scene,LoadSceneMode.Single);

        _=FadeOutVisual();

        //SeamlessSceneTransition.loadedScenes.Clear();
        /*
        SingleInScene[] toRemove = FindObjectsByType<SingleInScene>(FindObjectsSortMode.None);
        foreach (var r in toRemove)
        {
            r.gameObject.SetActive(false);
            Destroy(r);
        }*/

        sceneIsLoading = false;
        
    }
    public static async Awaitable TransitionScene(string scene)
    {
        int index = SceneManager.GetSceneByName(scene).buildIndex;
        await TransitionScene(index);
    }

    public static async Awaitable FadeInVisual()
    {
        for (float t = 0; t < 1f; t += _speed * Time.deltaTime)
        {
            transitionVisual.SetFloat("_Value", t);
            await Awaitable.NextFrameAsync();
        }
        transitionVisual.SetFloat("_Value", 1);
    }
    public static async Awaitable FadeOutVisual()
    {
        for (float t = 0; t < 1f; t += _speed * Time.deltaTime)
        {
            transitionVisual.SetFloat("_Value", 1f-t);
            await Awaitable.NextFrameAsync();
        }
        transitionVisual.SetFloat("_Value", 0);
    }




}
