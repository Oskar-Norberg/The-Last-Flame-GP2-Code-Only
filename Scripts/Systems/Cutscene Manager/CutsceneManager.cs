using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : Singleton<CutsceneManager>
{
    public delegate void OnCutsceneStart(PlayableDirector cutscene);
    public event OnCutsceneStart onCutsceneStart;
    
    public delegate void OnCutsceneFinished();
    public event OnCutsceneFinished onCutsceneFinished;

    Queue<PlayableDirector> _cutsceneQueue = new();
    private bool _isPlaying;

    private PlayableDirector _currentCutscene;

    public void PlayCutscene(PlayableDirector cutscene)
    {
        _cutsceneQueue.Enqueue(cutscene);

        // If a cutscene is already playing.
        if (_isPlaying)
            return;
        
        PlayableDirector cutsceneToPlay = _cutsceneQueue.Dequeue();
        cutsceneToPlay.Play();
        _currentCutscene = cutsceneToPlay;
        
        onCutsceneStart?.Invoke(cutsceneToPlay);
        
        _isPlaying = true;
        cutscene.stopped += CutsceneFinished;
    }

    private void CutsceneFinished(PlayableDirector cutscene)
    {
        _currentCutscene = null;
        cutscene.stopped -= CutsceneFinished;
        onCutsceneFinished?.Invoke();
        _isPlaying = false;

        if (!(_cutsceneQueue.Count > 0))
            return;
        
        PlayableDirector nextCutscene = _cutsceneQueue.Dequeue();
        PlayCutscene(nextCutscene);
    }
    
    public void SkipCutscene()
    {
        _currentCutscene.time = _currentCutscene.duration;
    }
}
