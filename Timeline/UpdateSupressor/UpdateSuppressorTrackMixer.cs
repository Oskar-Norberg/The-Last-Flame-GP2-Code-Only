using UnityEngine;
using UnityEngine.Playables;

public class UpdateSuppressorTrackMixer : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var cutsceneState = playerData as CutsceneState;

        if (cutsceneState == null) 
            return;

        cutsceneState.StopSuppressUpdates();
        
        int inputCount = playable.GetInputCount();
        
        // This shit's so nested, sorry lol - Oskar
        for (int i = 0; i < inputCount; i++)
        {
            if (!(playable.GetInputWeight(i) > 0f)) 
                continue;
            
            ScriptPlayable<UpdateSuppressorBehaviour> inputPlayable = (ScriptPlayable<UpdateSuppressorBehaviour>) playable.GetInput(i);
            
            UpdateSuppressorBehaviour input = inputPlayable.GetBehaviour();

            if (input == null) 
                continue;
            
            cutsceneState.StartSuppressUpdates();
            break;
        }
    }
}
