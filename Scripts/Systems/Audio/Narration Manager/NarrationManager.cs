using UnityEngine;

public class NarrationManager
{
    private AudioSource _audioSource;

    public NarrationManager(AudioSource audioSource)
    {
        _audioSource = audioSource;
    }

    public void PlayDialogueSound(AudioClip clip)
    {
        _audioSource.Stop();
        _audioSource.clip = clip;
        _audioSource.Play();
    }
}
