using UnityEngine;

public class ToggleEmission : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystem;
    
    public void SetActive(bool active)
    {
        ParticleSystem.EmissionModule emissionModule = particleSystem.emission;
        // TODO: Check if this just does it temporarily on the copied struct, I actually don't know
        emissionModule.enabled = active;
    }
}
