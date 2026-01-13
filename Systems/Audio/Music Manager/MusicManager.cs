using System.Collections;
using UnityEngine;

public class MusicManager
{
    private AudioSource _audioSource;
    private AudioClip _currentTrack;

    public MusicManager(AudioSource audioSource)
    {
        _audioSource = audioSource;
    }

    public void PlaySong(AudioClip clip)
    {
        _currentTrack = clip;
        _audioSource.Stop();
        _audioSource.clip = clip;
        _audioSource.Play();
    }

    public AudioClip GetCurrentTrack()
    {
        return _currentTrack;
    }
}
