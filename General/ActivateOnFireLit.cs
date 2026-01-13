using System;
using General.Interactable;
using UnityEngine;

public class ActivateOnFireLit : MonoBehaviour
{
    [SerializeField] private Campfire campfire;

    private void Update()
    {
        if (campfire.isLit)
        {
            gameObject.SetActive(true);
        }
    }
}
