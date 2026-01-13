using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class DialogueLog : Singleton<DialogueLog>
{
    public List<Log> Logs { get; private set; } = new();
    
    public delegate void OnDialogueLogUpdated(PlayableDirector playableDirector);
    public event OnDialogueLogUpdated onDialogueLogUpdated;

    private void Start()
    {
        CutsceneManager.Instance.onCutsceneStart += CutscenePlayed;
    }

    private void OnDestroy()
    {
        CutsceneManager.Instance.onCutsceneStart -= CutscenePlayed;
    }

    private void CutscenePlayed(PlayableDirector cutscene)
    {
        Logs.Add(CutsceneToLog(cutscene));
        onDialogueLogUpdated?.Invoke(cutscene);
    }

    private Log CutsceneToLog(PlayableDirector cutscene)
    {
        Log log = new();
        
        log.Name = cutscene.playableAsset.name;
        log.Subtitles = GetSubtitleSOsFromPlayableDirector(cutscene);
        
        return log;
    }

    private List<SubtitleSO> GetSubtitleSOsFromPlayableDirector(PlayableDirector playableDirector)
    {
        List<SubtitleSO> subtitleClipList = new();

        TimelineAsset timeline = playableDirector.playableAsset as TimelineAsset;

        if (timeline == null)
            return null;

        SubtitleTrack subtitleTrack = null;
       
        for (int i = 0; i < timeline.rootTrackCount; i++)
        {
            subtitleTrack = timeline.GetRootTrack(i) as SubtitleTrack;
            
            if (subtitleTrack != null) break; 
        }

        if (subtitleTrack == null)
        {
            Debug.LogWarning("Subtitle Track Not Found");
            return null;
        }

        IEnumerable<TimelineClip> timelineClips = subtitleTrack.GetClips();

        foreach (TimelineClip timelineClip in timelineClips)
        {
            SubtitleClip subtitleClip = timelineClip.asset as SubtitleClip;

            if (subtitleClip == null)
                continue;

            if (subtitleClip.subtitle == null)
                continue;
           
            subtitleClipList.Add(subtitleClip.subtitle);
        }

        return subtitleClipList;
    }
    
    private List<SubtitleSO> GetSubtitleSOsFromOneShotCutscene(OneShotCutscene oneShotCutscene)
    {
        return GetSubtitleSOsFromPlayableDirector(oneShotCutscene.Cutscene);
    }

    public class Log
    {
        public string Name;
        public List<SubtitleSO> Subtitles;
    }
}
