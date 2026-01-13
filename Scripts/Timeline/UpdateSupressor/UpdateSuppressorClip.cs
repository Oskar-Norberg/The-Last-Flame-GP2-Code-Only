using UnityEngine;
using UnityEngine.Playables;

public class UpdateSuppressorClip : PlayableAsset
{
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<UpdateSuppressorBehaviour>.Create(graph);
        
        return playable;
    }
}
