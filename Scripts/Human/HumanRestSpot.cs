using System;
using UnityEngine;

public class HumanRestSpot : MonoBehaviour
{
    public bool IsOccupied { get; private set; } = false;

    public bool IsReserved => (bool) _reserver;
    
    private Transform _reserver;
    
    private void OnTriggerEnter(Collider other)
    {
        if (IsOccupied || !IsReserved) return;

        if (_reserver != other.transform)
            return;
        
        if (!other.TryGetComponent<Human>(out Human human))
            return;
        
        Occupy(human);
    }

    public void Reserve(Transform reserver)
    {
        _reserver = reserver;
    }

    private void Occupy(Human human)
    {
        IsOccupied = true;
        human.Rest();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 1);
    }
}
