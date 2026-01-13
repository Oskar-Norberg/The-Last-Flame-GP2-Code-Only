using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

[RequireComponent(typeof(PlayableDirector))]
public abstract class OneShotCutscene : MonoBehaviour
{
    [Header("Do not use the PlayOnAwake from the playable director, it should always be false.")]
    [SerializeField] private bool playOnAwake = false;
    
    public PlayableDirector Cutscene => _cutscene;
    private PlayableDirector _cutscene;
    
    private bool _hasPlayed = false;

    private void Awake()
    {
        _cutscene = GetComponent<PlayableDirector>();
        ErrorCheckPlayOnAwake(_cutscene);
        
        LoadSave(_cutscene);
        
        if (playOnAwake)
            PlayCutscene();
    }

    protected void PlayCutscene()
    {
        if (_hasPlayed) return;
        _hasPlayed = true;

        Save(_cutscene);

        CutsceneManager.Instance.PlayCutscene(_cutscene);
    }

    private void ErrorCheckPlayOnAwake(PlayableDirector playableDirector)
    {
        if (!playableDirector.playOnAwake) 
            return;
        
        playableDirector.playOnAwake = false;
        Debug.LogWarning("Do not use the PlayOnAwake from the playable director, it should always be false.");
    }

    private void Save(PlayableDirector playableDirector)
    {
        try
        {
            int index = Array.IndexOf(SaveSystem.allDialogs.cutscenes, (TimelineAsset) _cutscene.playableAsset);
            SaveSystem.save.unlock_dialog[index] = true;
            SaveSystem.Save();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private void LoadSave(PlayableDirector playableDirector)
    {
        // Ignore the error if cutscene is not saved.
        
        try
        {
            int index = Array.IndexOf(SaveSystem.allDialogs.cutscenes, (TimelineAsset) playableDirector.playableAsset);

            _hasPlayed = SaveSystem.save.unlock_dialog[index];
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
