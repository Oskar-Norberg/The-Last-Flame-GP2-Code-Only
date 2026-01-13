using System.Collections.Generic;
using UnityEngine;

public class HumanRestSpace : MonoBehaviour
{
    // The order really doesn't matter, so I'll make this a stack for now.
    private Stack<HumanRestSpot> _humanRestSpots = new Stack<HumanRestSpot>();
    
    private int SpotsLeft => _humanRestSpots.Count;
    public int HumansResting { get; private set; }

    private void Awake()
    {
        List<HumanRestSpot> restSpotsList = FindChildRestSpots();
        AddListToStack(ref _humanRestSpots, restSpotsList);
    }

    public HumanRestSpot GetRandomRestSpot(Transform reserver)
    {
        if (SpotsLeft == 0)
        {
            Debug.LogWarning("No HumanRestSpots left");
            return null;
        }

        HumanRestSpot restSpot = _humanRestSpots.Pop();
        restSpot.Reserve(reserver);

        HumansResting++;
        
        return restSpot;
    }
    
    private List<HumanRestSpot> FindChildRestSpots()
    {
        List<HumanRestSpot> humanRestSpots = new List<HumanRestSpot>();
        
        for (int i = 0; i < transform.childCount; i++)
        {
            if (!transform.GetChild(i).TryGetComponent<HumanRestSpot>(out var restSpot))
                continue;
            
            humanRestSpots.Add(restSpot);
        }

        return humanRestSpots;
    }

    private void AddListToStack<T>(ref Stack<T> stack, List<T> list)
    {
        foreach (T item in list)
        {
            stack.Push(item);
        }
    }
}
