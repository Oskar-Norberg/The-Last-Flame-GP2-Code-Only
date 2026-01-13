using System.Collections.Generic;
using General.Interactable;
using General.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Enemy
{
    public class IceWraith : Enemy
    {
        [SerializeField] private float damageRange = 15;
        [SerializeField] private Transform mesh;
        [SerializeField] private int damage = 1;
        [SerializeField] private float floatRadius = 10;
        private List<Interactable> _firesOrQuestpointsNearby = new List<Interactable>();

        [Header("Frost UI image to appear when the player gets damaged\n" +
                "Not assigning this variable will not result in an error.")]
        [SerializeField] private Image frostUI;

        private PlayerEntity _playerEntity;
        private Vector3 _wanderTarget;
        private Vector3 _originalPosition;
        private bool _overlayEnabled;
        private float _overlayAlpha;
        private float _timeSinceTarget;
        private Vector3 _fireAvoidVector;
        

        protected override void Start()
        {
            base.Start();

            _playerEntity = player.GetComponent<PlayerEntity>();
            rigidbody.useGravity = false;
            _originalPosition = transform.position;
            _overlayEnabled = frostUI != null;
            
            moveTowardsPlayer = false;
            attackPlayer = false;
            
            Vector3 offset = new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 1f), Random.Range(-1f, 1f)).normalized;

            _wanderTarget = _originalPosition + offset * floatRadius;

            foreach (var fire in Object.FindObjectsByType<Campfire>(FindObjectsSortMode.None))
            {
                _firesOrQuestpointsNearby.Add(fire);
            }
            
            foreach (var fire in Object.FindObjectsByType<Torch>(FindObjectsSortMode.None))
            {
                _firesOrQuestpointsNearby.Add(fire);
            }
            
            foreach (var fire in Object.FindObjectsByType<CheckPoint>(FindObjectsSortMode.None))
            {
                _firesOrQuestpointsNearby.Add(fire);
            }
        }

        protected override void Update()
        {
            base.Update();
            
            frostUI.color = new Color(frostUI.color.r, frostUI.color.g, frostUI.color.b, 0);
            
            mesh.rotation *= Quaternion.Euler(0, 0, Time.deltaTime * 360);

            Interactable nearestFire = null;

            foreach (var fire in _firesOrQuestpointsNearby)
            {
                if (fire.interacted)
                {
                    if (nearestFire == null || Vector3.Distance(nearestFire.transform.position, transform.position) > Vector3.Distance(fire.transform.position, transform.position))
                    {
                        nearestFire = fire;
                    }
                }
            }

            if (nearestFire == null) return;

            if (Vector3.Distance(nearestFire.transform.position, transform.position) < damageRange + 3)
            {
                _fireAvoidVector = (transform.position - nearestFire.transform.position).normalized;
            }
            else
            {
                _fireAvoidVector = Vector3.zero;
            }
        }

        protected override void OnDetectionEnter()
        {
            moveTowardsPlayer = true;
            attackPlayer = true;
        }

        protected override void OnDetectionStay()
        {
            if (Vector3.Distance(player.position, transform.position) < damageRange)
            {
                _overlayAlpha += Time.deltaTime;
                _overlayAlpha = Mathf.Clamp01(_overlayAlpha);
            }
            else
            {
                _overlayAlpha -= Time.deltaTime;
                _overlayAlpha = Mathf.Clamp01(_overlayAlpha);
            }

        }

        protected override void OnLossEnter()
        {
            moveTowardsPlayer = false;
            attackPlayer = false;

            health = maxHealth;
        }

        protected override void OnLossStay()
        {
            if ((transform.position - _wanderTarget).magnitude < 1 || _timeSinceTarget > floatRadius)
            {
                Vector3 offset = new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 1f), Random.Range(-1f, 1f)).normalized;

                _wanderTarget = _originalPosition + offset * floatRadius;

                _timeSinceTarget = 0;
            }
            
            _overlayAlpha -= Time.deltaTime;
            _overlayAlpha = Mathf.Clamp01(_overlayAlpha);

        }

        protected override void LateUpdate()
        {
            Vector3 plv;

            if (_fireAvoidVector != Vector3.zero) plv = _fireAvoidVector;
            else if (!moveTowardsPlayer) plv = (_wanderTarget - transform.position).normalized;
            else plv = (player.position - transform.position).normalized;
            float m = movementSpeed;

            rigidbody.linearVelocity = plv * m;

            if (_overlayEnabled)
            {
                frostUI.color += new Color(0, 0, 0, _overlayAlpha);
            }
            _timeSinceTarget += Time.deltaTime;
        }

        protected override void OnAttack()
        {
            if (Vector3.Distance(player.position, transform.position) < damageRange)
            {
                _playerEntity.TakeDamage(Mathf.RoundToInt(damage / Mathf.Clamp(Vector3.Distance(player.position, transform.position), 1, damage)));
            }
        }
        
        protected override void OnHit()
        {
            Debug.Log("ouch");
        }

        protected override void Die()
        {
            Debug.Log("x_x");
        }
    }
}

