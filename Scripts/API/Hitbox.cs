using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Hitbox : MonoBehaviour
{
    public Entity owner;
    protected new Collider collider;

    public int damage;

    [Tooltip("The time between being able to hit the same creature twice with the same hitbox.")]
    public float timeBetweenHits = 0.25f;

    public bool canEnter = true;
    public bool canStay = false;
    public bool canExit = false;

    public UnityEvent<Entity> onEnter;
    public UnityEvent<Entity> onExit;

    private HashSet<Entity> _isImmune = new HashSet<Entity>();


    protected virtual void Start()
    {
        collider = GetComponent<Collider>();
    }
    public virtual void SetOwner(Entity self)
    {
        owner = self;
    }
    protected virtual void Update()
    {}
    protected virtual void OnValidate()
    {
        gameObject.layer = 8; //Hitbox layer
        collider = gameObject.GetComponent<Collider>();
        collider.isTrigger = true;
    }
    

    protected void Hit(Entity e)
    {
        if(_isImmune.Contains(e)) return;
        e.TakeDamage(damage);
        if(owner != null) owner.DealDamage(damage);
        _ = Immunity(e, timeBetweenHits);

        OnHit(e);
    }

    //override as a child to add additional functionality 
    protected virtual void OnHit(Entity e)
    {}

    //CanHit is responsible for faction logic
    protected virtual bool CanHit(Entity toHit)
    {
        return toHit.isEnemy != owner.isEnemy;     //friendly fire off
        //return toHit.isEnemy == owner.isEnemy;   //friendly fire only
        //return true;                              //friendly and enemy fire 
        //return false;                             //dont do this...
    }
    private async Awaitable Immunity(Entity e, float wait)
    {
        _isImmune.Add(e);
        await Awaitable.WaitForSecondsAsync(wait);
        _isImmune.Remove(e);
    }


    protected virtual void Enter(Entity e)
    {
        onEnter.Invoke(e);
        if (CanHit(e))
        {
            Hit(e);
        }
    }
    protected virtual void Stay(Transform e)
    { }
    protected virtual void Exit(Entity e)
    {
        onExit.Invoke(e);
    }

    #region Object pooling (not sure if we will use :P)
    public virtual void UseHitbox()
    {
        gameObject.SetActive(true);
        enabled = true;
        collider.enabled = true;
    }
    public virtual void ResetHitbox()
    {
        _isImmune.Clear();
    }
    #endregion

    #region Collision

    private void OnTriggerEnter(Collider other)
    {
        if (!canEnter) return;
        var entity = other.GetComponent<Entity>();
        if (entity != null)
        {
            Enter(entity);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (!canStay) return;
        Stay(other.transform);
    }
    private void OnTriggerExit(Collider other)
    {
        if(!canExit) return;
        var entity = other.GetComponent<Entity>();
        if(entity != null)
        {
            Exit(entity);
        }
    }

    #endregion
}
