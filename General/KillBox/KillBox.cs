using System;
using General.Player;
using UnityEngine;

public class KillBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out Player player))
            return;
        
        player.PlayerEntity.TakeDamage(Int32.MaxValue);
    }
}
