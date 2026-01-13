using UnityEngine;

public class Freezable : MonoBehaviour
{
    public bool IsFrozen => isFrozen;

    [SerializeField] private bool isFrozen;
    
    public void Freeze()
    {
        isFrozen = true;
    }

    public void Thaw()
    {
        isFrozen = false;
    }
}
