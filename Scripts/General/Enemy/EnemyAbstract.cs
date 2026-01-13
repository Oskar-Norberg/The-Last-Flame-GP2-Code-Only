using System;
using General.Player;
using UnityEngine;
using UnityEngine.Events;

namespace Enemy
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public abstract class Enemy : Entity
    {
        // PRIVATE VARIABLES
        
        private protected Rigidbody rigidbody;
        private protected Transform player;
        private bool _detectedPlayer;
        private protected bool moveTowardsPlayer;
        private protected bool attackPlayer;
        private float _timeSinceLastAttack;
        private protected Collider collider;
        
        [Header("The function OnHit() will be added automatically into the event.\n" +
                "There is no need to manually add it.")]
        [Space]
        [Header("If you're confused about anything below, hover the mouse over the variable,\n" +
                "some of them have helpful tooltips.\n" +
                "If you're still confused, ask me (Anton Lanning) for help!")]
        [Space]
        
        [Header("SUPER IMPORTANT! The enemy currently detects the player if it's nearby\n" +
                "something with the tag \"Player\". You need to make sure every child to the\n" +
                "player has the tag \"Player\".")]
        [Space]
        
        

        // SERIALIZED VARIABLES
        [SerializeField] private protected float movementSpeed = 3;
        [Space]
        [SerializeField] private float detectionRange = 5;
        [Tooltip("When true, there has to be a clear line of sight for the enemy to detect the player")]
        private bool useRaycast = false;
        [SerializeField] private float attackCooldown = 1;
        
        // PROTECTED FUNCTIONS
        protected abstract void OnDetectionEnter();
        protected abstract void OnDetectionStay();

        protected abstract void OnLossEnter();
        protected abstract void OnLossStay();
        protected abstract void OnAttack();
        protected abstract void OnHit();
        
        // PRIVATE FUNCTIONS
        protected virtual void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            rigidbody.constraints = (RigidbodyConstraints)80;
            player = Player.Instance.transform;
            collider = GetComponent<Collider>();

            collider.material.staticFriction = 0;
            collider.material.dynamicFriction = 0;
            
            onDamage.AddListener(OnHit);
        }

        protected virtual void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;
            
            if (attackPlayer)
            {
                if (_timeSinceLastAttack >= attackCooldown)
                {
                    OnAttack();
                    _timeSinceLastAttack = 0;
                }
            }
            
            if (Vector3.Distance(transform.position, player.position) <= detectionRange)
            {
                if (useRaycast)
                {
                    RaycastHit hit;
                    Vector3 playerLookVector = (player.position - transform.position).normalized;
                    
                    LayerMask lm = LayerMask.NameToLayer("Enemy");
                    
                    if (!Physics.Raycast(transform.position, playerLookVector, out hit, detectionRange, ~lm)
                        || !hit.transform.CompareTag("Player"))
                    {
                        if (_detectedPlayer) OnLossEnter();

                        _detectedPlayer = false;
                        OnLossStay();

                        return;
                    }
                }
                
                
                if (!_detectedPlayer) OnDetectionEnter();

                _detectedPlayer = true;
                OnDetectionStay();

                return;
            }
            
            if (_detectedPlayer) OnLossEnter();
            
            _detectedPlayer = false;
            OnLossStay();

        }

        protected virtual void LateUpdate()
        {
            if (!moveTowardsPlayer) return;
            
            Vector3 plv = (player.position - transform.position).normalized;
            float m = movementSpeed * Time.deltaTime;

            rigidbody.linearVelocity = new Vector3(plv.x * m, rigidbody.linearVelocity.y, plv.z * m);
        }
    }
}

