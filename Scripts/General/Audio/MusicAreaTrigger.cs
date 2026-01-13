using System;
using General.Player;
using UnityEngine;

public class MusicAreaTrigger : MusicTrigger
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out var playerMovement))
        {
            print("test");
            PlayTrack();
        }
    }
}
