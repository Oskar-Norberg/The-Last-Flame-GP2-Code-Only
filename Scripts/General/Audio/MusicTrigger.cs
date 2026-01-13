using UnityEngine;

public abstract class MusicTrigger : MonoBehaviour
{
    [SerializeField] protected AudioClip track;

    protected void PlayTrack()
    {
        AudioClip currentTrack = AudioManager.Instance.MusicManager.GetCurrentTrack();
        if (currentTrack != null && currentTrack == track) 
            return;
        
        AudioManager.Instance.MusicManager.PlaySong(track);
    }
}
