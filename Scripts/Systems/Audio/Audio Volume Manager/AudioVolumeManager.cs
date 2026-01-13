using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioVolumeManager
{
    public enum AudioGroups { Master, Music, Dialogue, SFX, Ambience }
    
    [SerializeField] private AudioMixer _audioMixer;

    public AudioVolumeManager(AudioMixer audioMixer)
    {
        _audioMixer = audioMixer;
    }
    
    public void SetVolume(AudioGroups audioGroup, float db)
    {
        string parameterName = audioGroup.ToString() + "Volume";
        _audioMixer.SetFloat(parameterName, db);
    }
}