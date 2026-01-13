using System;
using General.Player;
using UnityEngine;

public class KillFloor : MonoBehaviour
{
    [SerializeField] private float killFloorY;

    private void Update()
    {
        if (Player.Instance.transform.position.y < killFloorY)
        {
            Player.Instance.PlayerEntity.TakeDamage(Int32.MaxValue);
        }
    }
}
