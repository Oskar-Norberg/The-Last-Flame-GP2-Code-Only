using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackBindingType(typeof(CutsceneState))]
[TrackClipType(typeof(UpdateSuppressorClip))]
public class UpdateSuppressorTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<UpdateSuppressorTrackMixer>.Create(graph, inputCount);
    }
}
