using UnityEngine;
using UnityEngine.Events;

public abstract class Entity : MonoBehaviour
{
    public int maxHealth = 1;
    [HideInInspector] public int health;
    private bool _isDead = false; 

    public bool isEnemy;

    public UnityEvent onDamage;

    protected virtual void Awake()
    {
        health = maxHealth;
    }

    //Die is the funciton called when your health reaches 0, this has to be defined
    protected abstract void Die();
    public bool IsDead() => _isDead;

    #region Damage Functions

    public virtual void TakeDamage(int damage)
    {
        if(_isDead) return;
        onDamage.Invoke();
        health -= damage;
        if (health <= 0)
        {
            _isDead = true;
            Die();
        }
    }
    public virtual void DealDamage(int damage)
    {
        if(_isDead) return;

        //Only for overrides. Use if you have an enemy that heals when it deals damage or any such cases.
    }

    #endregion
}
