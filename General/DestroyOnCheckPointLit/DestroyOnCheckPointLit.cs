using System;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnCheckPointLit : MonoBehaviour
{
    [SerializeField] private List<CheckPoint> checkPoints;
    
    private uint _checkPointsLitCount = 0;

    private void OnEnable()
    {
        foreach (CheckPoint checkPoint in checkPoints)
            checkPoint.OnCheckpointLit += DestroyGameObject;
    }
    
    private void OnDisable()
    {
        foreach (CheckPoint checkPoint in checkPoints)
            checkPoint.OnCheckpointLit -= DestroyGameObject;
    }
    
    private void DestroyGameObject()
    {
        if (++_checkPointsLitCount >= checkPoints.Count)
            Destroy(gameObject);
    }
}
