using System;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerEntity : Entity
{
    [SerializeField] private Bar hpBar;
    private float shakeIntensity = 1;
    
    private bool _hasHpBar;
    private CinemachineBasicMultiChannelPerlin _shake;
    private bool _shakeAvailable;
    private float _shakeTimer;
    [SerializeField] private FoxFire visualComponant;
    private bool _hasFoxFire;
    
    public delegate void OnDeathEventDelegate();
    public event OnDeathEventDelegate OnDeath;

    public void Start()
    {
        visualComponant = GetComponentInChildren<FoxFire>();
        visualComponant.enabled = true;
        if (hpBar != null) _hasHpBar = true;
        _shake = FindAnyObjectByType<CinemachineBasicMultiChannelPerlin>();
        if (_shake != null)
        {
            _shakeAvailable = true;
            _shake.AmplitudeGain = 0;
        }
        if (visualComponant != null) _hasFoxFire = true;
    }

    protected override void Die()
    {
        
        //play death animation

        //wait till finished, maybe make die an awaiable or make it call an awaitable?

        
        //_=SaveSystem.LoadLevel();

        //Debug.Log("The player just fucking died, we should probably do something about that");
        //I did something about that :3 - milo
        // I did something else about it :3 - oskar
        OnDeath?.Invoke();
    }

    public override void TakeDamage(int damage)
    {
        if (_shakeAvailable)
        {
            _shake.AmplitudeGain += (float)damage / 10 * shakeIntensity;
            _shakeTimer += 1;
        }
        base.TakeDamage(damage);

        if (_hasFoxFire) visualComponant.value = ((float)health / (float)maxHealth);
    }

    private void Update()
    {
        if (_hasHpBar) hpBar.SetBar((float)health / maxHealth);
        
        if (_shakeAvailable)
        {
            if (_shakeTimer > 0)
            {
                _shakeTimer -= Time.deltaTime;

                if (_shakeTimer <= 0)
                {
                    _shake.AmplitudeGain = 0;
                }
            } 
        }
    }
}
