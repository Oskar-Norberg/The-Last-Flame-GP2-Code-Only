using System.Collections.Generic;
using UnityEngine;

public class HumanManager : Singleton<HumanManager>, IUpdateable
{
    [Header("Time to wait between ticking a humans behaviour tree")]
    [Tooltip("Negative effects start appearing if value too high. Around 10ms is fine.")]
    [SerializeField] private float msBetweenUpdates = 10;

    private List<Human> _humans = new();

    private float _msSinceLastUpdate = 0;
    private int _currentHumanIndex = 0;

    private void OnEnable()
    {
        Human.OnHumanInstantiated += AddHuman;
        Human.OnHumanDestroyed += RemoveHuman;
    }

    private void OnDisable()
    {
        Human.OnHumanInstantiated -= AddHuman;
        Human.OnHumanDestroyed -= RemoveHuman;
    }

    public void CustomUpdate(float deltaTime)
    {
        if (_humans.Count == 0)
            return;
        
        _msSinceLastUpdate += deltaTime * 1000;

        if (_msSinceLastUpdate < msBetweenUpdates)
            return;

        if (_currentHumanIndex >= _humans.Count)
            _currentHumanIndex = 0;

        _msSinceLastUpdate = 0;

        _humans[_currentHumanIndex++].CustomUpdate(deltaTime);
    }

    public void CustomFixedUpdate(float fixedDeltaTime)
    {
        foreach (Human human in _humans)
        {
            human.CustomFixedUpdate(fixedDeltaTime);
        }
    }

    private void AddHuman(Human human)
    {
        _humans.Add(human);
    }

    private void RemoveHuman(Human human)
    {
        _humans.Remove(human);
    }
}