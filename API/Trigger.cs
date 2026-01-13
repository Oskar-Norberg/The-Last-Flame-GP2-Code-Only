using UnityEngine;

public abstract class Trigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        OnEnter();
    }
    private void OnTriggerStay(Collider other)
    {
        OnStay();
    }
    private void OnTriggerExit(Collider other)
    {
        OnExit();
    }

    protected abstract void OnEnter();
    protected virtual void OnStay() { }
    protected virtual void OnExit() { }
}
