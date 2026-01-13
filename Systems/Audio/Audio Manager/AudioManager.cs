using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : PersistentSingleton<AudioManager>
{
    public AudioVolumeManager AudioVolumeManager { get; private set; }
    public MusicManager MusicManager { get; private set; }
    public NarrationManager NarrationManager { get; private set; }

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource narrationAudioSource;
    
    private new void Awake()
    {
        base.Awake();
        
        AudioVolumeManager = new AudioVolumeManager(audioMixer);
        MusicManager = new MusicManager(musicAudioSource);
        NarrationManager = new NarrationManager(narrationAudioSource);
    }
}
