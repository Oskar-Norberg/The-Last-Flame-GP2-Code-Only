using UnityEngine;
using UnityEngine.Playables;

public class SubtitleTrackMixer : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var subtitleManager = playerData as SubtitleManager;

        if (subtitleManager == null) 
            return;

        string text = string.Empty;
        Color color = Color.clear;
        subtitleManager.SetText(text, color, 0f);
        
        int inputCount = playable.GetInputCount();
        float inputWeight = 0f;

        SubtitleSO subtitle = null;
        
        for (int i = 0; i < inputCount; i++)
        {

            if (playable.GetInputWeight(i) > 0f)
            {
                inputWeight = playable.GetInputWeight(i);
                
                ScriptPlayable<SubtitleBehaviour> inputPlayable = (ScriptPlayable<SubtitleBehaviour>) playable.GetInput(i);
            
                SubtitleBehaviour input = inputPlayable.GetBehaviour();
                subtitle = input.subtitle;

                if (subtitle)
                    break;
            }
        }

        if (!subtitle) return;
        
        subtitleManager.SetText(subtitle.text, subtitle.color, inputWeight);
        subtitleManager.UpdateTextBoxSize();
    }
}
