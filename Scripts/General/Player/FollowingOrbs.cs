using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace General.Player
{
    
    [RequireComponent(typeof(PlayerController))]
    public class FollowingOrbs : MonoBehaviour
    {
        // Public variables
        
        public int OrbCount { get; private set; }
        public int MaximumOrbCount { get; private set; }
        
        [Header("If you're confused about anything, hover the mouse over the variable,\n" +
                "some of them have helpful tooltips.\n" +
                "If you're still confused, ask me (Anton Lanning) for help!")]
        [Space]
        [Header("Despite the name of this component, it should be placed inside the PLAYER")]
        
        // Serialized variables
        [Tooltip("If this is not set then it will generate orbs automatically, however this needs to be" +
                 "set in the final game.")]
        [Space]
        [SerializeField] private GameObject orbPrefab;
        [Space] 
        [SerializeField] private int startOrbCount = 3;
        [SerializeField] private int maxOrbCount = 5;
        [SerializeField] private float orbSpacing = 1;
        [SerializeField] private float yOffset = 1;

        // Private variables

        private List<Transform> _orbs = new List<Transform>();
        private List<Transform> _orbpool = new List<Transform>();
        private float _orbSpeed = 3;
        private PlayerController _playerController;

        // Public functions
        
        public bool AddOrb(Vector3 spawnPosition)   // Function that adds an orb to the player.
                                                    // Returning false if the player has reached the maximum orb count.
        {
            if (OrbCount == MaximumOrbCount) return false;

            Transform newOrb;
            Vector3 orbPosition = spawnPosition;
            OrbCount++;
            
            if (_orbpool.Count > 0)
            {
                newOrb = _orbpool[0];
                _orbpool.RemoveAt(0);
                newOrb.position = orbPosition;
            }
            else
            {
                newOrb = Instantiate(orbPrefab, orbPosition, Quaternion.identity).transform;
            }
            
            _orbs.Add(newOrb);
            newOrb.gameObject.SetActive(true);
            
            return true;
        }

        public bool AddOrb() // A simpler function in case there is no spawn position.
        {
            return AddOrb(GetOrbTarget(OrbCount));
        }
        
        public void AddOrbs(int orbCount) // A simple function for adding several orbs. This will not return anything
                                          // no matter if it has reached the maximum or not.
        {
            for (int i = 0; i < orbCount; i++)
            {
                AddOrb();
            }
        }

        public bool ConsumeOrb(Vector3 moveTarget, float moveTime, UnityAction callWhenFinished)    
            // Function that removes an orb from the player. Returning false if the player doesn't have any orbs.
            // This function has a field for a method that can be called when moving is finished.
        {
            if (OrbCount == 0) return false;

            Transform consumedOrb = ConsumeAndGetConsumedOrb();

            StartCoroutine(MoveOrb(consumedOrb, moveTarget, moveTime, true, callWhenFinished));
            
            return true;
        }
        
        public bool ConsumeOrb(Vector3 moveTarget, float moveTime)    
            // Simplified function if no method should be called when moving is finished.
        {
            if (OrbCount == 0) return false;

            Transform consumedOrb = ConsumeAndGetConsumedOrb();

            StartCoroutine(MoveOrb(consumedOrb, moveTarget, moveTime, false, null));
            
            return true;
        }
        
        public bool ConsumeOrb()    
            // Simplified function if the orb should just disappear.
        {
            if (OrbCount == 0) return false;

            Transform orb = ConsumeAndGetConsumedOrb();

            orb.gameObject.SetActive(false);
            _orbpool.Add(orb);
            
            return true;
        }

        public void IncreaseMaxOrbs()
        {
            IncreaseMaxOrbsBy(1);
        }

        public void IncreaseMaxOrbsBy(int count)
        {
            SetMaxOrbsTo(MaximumOrbCount + count);
        }

        public void SetMaxOrbsTo(int newMaxValue)
        {
            MaximumOrbCount = newMaxValue;
        }
        
        // Private functions
        
        private void Start()
        {
            _playerController = GetComponent<PlayerController>();
            OrbCount = startOrbCount;
            MaximumOrbCount = maxOrbCount;

            OrbCount = SaveSystem.save.orbCount;
            SetMaxOrbsTo(SaveSystem.save.maxOrbCount);
            
            if (orbPrefab == null) GenerateOrb();

            for (int i = 0; i < OrbCount; i++)
            {
                Vector3 orbPosition = GetOrbTarget(i);
                
                _orbs.Add(Instantiate(orbPrefab, orbPosition, Quaternion.identity).transform);
                _orbs[i].gameObject.SetActive(true);
            }
        }

        private void Update()
        {
            _orbSpeed = _playerController.CurrentPlayerSpeed;
            
            for (int i = 0; i < OrbCount; i++)
            {
                Vector3 orbTarget = GetOrbTarget(i);
                Vector3 lookVector = (orbTarget - _orbs[i].position) == Vector3.zero ? 
                    Vector3.zero : (orbTarget - _orbs[i].position).normalized;

                if (lookVector != Vector3.zero)
                    _orbs[i].rotation = Quaternion.LookRotation(lookVector, Vector3.up);

                if (Vector3.Distance(_orbs[i].position, orbTarget) > 0.2f)
                    _orbs[i].position += _orbs[i].forward *= _orbSpeed * Time.deltaTime;

            }
        }

        private Vector3 GetOrbTarget(int i)
        {
            if (i == 0) return transform.position - transform.forward * orbSpacing + Vector3.up * yOffset;
            
            return _orbs[i - 1].position - transform.forward * orbSpacing;
        }

        private Transform ConsumeAndGetConsumedOrb()
        {
            Transform orbToConsume = _orbs[0];

            _orbs.RemoveAt(0);
            OrbCount--;
            
            return orbToConsume;
        }

        private IEnumerator MoveOrb(Transform orb, Vector3 target, float time, bool runMethod, UnityAction method)
        {
            Vector3 originPosition = orb.position;
            
            for (float i = 0; i < 1; i += Time.deltaTime / time)
            {
                orb.position = Vector3.Lerp(originPosition, target, i);
                
                yield return null;
            }
            
            orb.gameObject.SetActive(false);
            _orbpool.Add(orb);
            
            if (runMethod) method.Invoke();
        }

        private void GenerateOrb() // THis function is only called if the orb prefab isn't set
        {
            GameObject newOrb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            newOrb.GetComponent<SphereCollider>().enabled = false;
            newOrb.transform.localScale = Vector3.one * 0.25f;
            orbPrefab = newOrb;
            newOrb.SetActive(false);
        }
    }
}
