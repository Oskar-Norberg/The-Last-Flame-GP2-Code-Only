using System;
using UnityEngine;

public class PlayMusicOnAwake : MonoBehaviour
{
    [SerializeField] AudioClip audioClip;

    private void Awake()
    {
        AudioManager.Instance.MusicManager.PlaySong(audioClip);
    }
}
