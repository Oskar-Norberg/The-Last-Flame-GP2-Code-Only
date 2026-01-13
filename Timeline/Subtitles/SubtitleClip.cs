using UnityEngine;
using UnityEngine.Playables;

public class SubtitleClip : PlayableAsset
{
    public SubtitleSO subtitle;
    
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<SubtitleBehaviour>.Create(graph);
        
        var subtitleBehaviour = playable.GetBehaviour();
        subtitleBehaviour.subtitle = subtitle;

        return playable;
    }
}
