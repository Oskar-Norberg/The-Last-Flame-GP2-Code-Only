using System.Collections.Generic;
using UnityEngine;

public class IsCloseToRestSpace : Decorator
{
    private Transform _origin;
    private HumanRestSpace[] _restSpaces;
    private float _detectionRange;
    
    public IsCloseToRestSpace(Transform origin, float detectionRange, Node child) : base(child)
    {
        _origin = origin;
        _detectionRange = detectionRange;
        
        _restSpaces = Object.FindObjectsByType<HumanRestSpace>(FindObjectsSortMode.None);
    }

    public override ReturnValue Evaluate()
    {
        HumanRestSpace closestRestSpace = FindNearest(_restSpaces, _origin.position);

        if (!(Vector3.Distance(closestRestSpace.transform.position, _origin.position) < _detectionRange))
            return ReturnValue.Failure;
        
        BlackBoard.SetData("ClosestRestSpace", closestRestSpace);
        
        return Child.Evaluate();
    }

    private T FindNearest<T>(T[] arr, Vector3 originPosition) where T : MonoBehaviour
    {
        float nearestDistance = float.MaxValue;
        T closest = null;

        foreach (T item in arr)
        {
            if (Vector3.Distance(originPosition, item.transform.position) < nearestDistance)
                closest = item;
        }

        return closest;
    }
}
