using System;
using System.Collections.Generic;
using General.Interactable;
using General.Item;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace General.Player
{
    public class NearestFireIndicator : MonoBehaviour
    {
        [SerializeField] private float hintDistance = 5;
        [SerializeField] private float hintCooldown = 5;
        [SerializeField] private float hintSpeed = 2;
        [SerializeField] private float yOffset = 2;
        [SerializeField] private GameObject hintVFXPrefab;
        
        private InputSystem_Actions isa;
        private InputAction hintAction;
        private float cooldownTimer;

        private Campfire[] allCampfires;
        private Torch[] allTorches;
        
        private void OnEnable()
        {
            isa = new InputSystem_Actions();
            hintAction = isa.Player.Hint;
            hintAction.Enable();
            hintAction.started += GetHint;

            allCampfires = FindObjectsByType<Campfire>(FindObjectsSortMode.None);
            allTorches = FindObjectsByType<Torch>(FindObjectsSortMode.None);

            if (hintVFXPrefab == null)
            {
                hintVFXPrefab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                hintVFXPrefab.GetComponent<Collider>().enabled = false;
                hintVFXPrefab.SetActive(false);
            }

        }

        private void OnDisable()
        {
            hintAction.Disable();
        }

        private void Update()
        {
            if (cooldownTimer > 0)
                cooldownTimer -= Time.deltaTime;
        }

        private void GetHint(InputAction.CallbackContext context)
        {
            if (cooldownTimer > 0) return;

            cooldownTimer = hintCooldown;
            
            List<Transform> unlit = new List<Transform>();
            Transform nearest = null;
            
            foreach (var campfire in allCampfires)
            {
                if (!campfire.isLit) unlit.Add(campfire.transform);
            }
            
            foreach (var torch in allTorches)
            {
                if (!torch.isLit) unlit.Add(torch.transform);
            }

            foreach (var fire in unlit)
            {
                if (nearest == null || Vector3.Distance(fire.position, transform.position) <
                    Vector3.Distance(nearest.position, transform.position))
                {
                    nearest = fire;
                }
            }
            
            Lead(nearest.position);
        }

        private async void Lead(Vector3 target)
        {
            Vector3 spawnPos = transform.position + Vector3.up * yOffset;
            target += Vector3.up * yOffset;
            
            Transform hintObject = Instantiate(hintVFXPrefab, spawnPos,
                Quaternion.LookRotation((target - spawnPos).normalized, Vector3.up)).transform;
            hintObject.gameObject.SetActive(true);
            
            for (float i = 0; i < hintDistance / hintSpeed; i += Time.deltaTime)
            {
                if (Vector3.Distance(hintObject.position, target) > 0.2f)
                {
                    hintObject.position += hintObject.forward * Time.deltaTime * hintSpeed;
                }
                
                
                await Awaitable.NextFrameAsync();
            }
            
            Destroy(hintObject.gameObject);
        }
    }
}
