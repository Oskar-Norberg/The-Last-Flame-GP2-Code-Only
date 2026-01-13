using System;
using System.Collections;
using General.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace General.Item
{
    [RequireComponent(typeof(Collider))]
    public abstract class Item : MonoBehaviour
    {
        [Header("If you're confused about anything, hover the mouse over the variable,\n" +
                "some of them have helpful tooltips.\n" +
                "If you're still confused, ask me (Anton Lanning) for help!")]
        [Space]
        
        [Header("INSTRUCTIONS FOR PROGRAMMERS (And anyone identifying as one):")]
        [Header("When the player tries to use an item, it calls a abstract function called\n" +
                "ActivateCheck(ItemPickup player). When you inherit, you need to override\n" +
                "the function to write the conditions on when the item can be activated. Inside\n" +
                "the function, make an if check, to make sure all conditions are met, and then\n" +
                "return true if they're met, or false if they're not. When it has returned true,\n" +
                "the player will automatically call the Activate(ItemPickup player) function.")]
        [Header("The Activate(ItemPickup player) function is also abstract, so just override it\n" +
                "and write what you want should happen when the item is activated.")]
        [Space]
        
        // PRIVATE VARIABLES THAT CAN BE CHANGED IN THE INSPECTOR
        
        [Tooltip("This setting will make the item activate when picked up. " +
                 "The item will be activated even if the player is carrying another item.")]
        [SerializeField] private bool activateInstantly;
        [SerializeField] private bool canBeDropped = true;
        [Tooltip("Seconds after the item has been dropped and that it can be picked up again.")]
        [SerializeField] private float pickupCooldown = 3;

        [Header("These settings only apply when the fox has the item in its mouth.\n" +
                "Read the tooltip on updateInRealtime for help setting the item up!")]
        [SerializeField] private Vector3 positionOffset;
        [SerializeField] private Quaternion rotation;
        [SerializeField] private float scale = 1;
        [Tooltip("This one is more performance-heavy, and should only be turned on when setting up the item.\n" +
                 "A recommendation for best results is to turn this on, experiment with the positionOffset, " +
                 "rotation and scale while the game is running, and when you're satisfied, write the values down " +
                 "somewhere and then enter them after the game is stopped. Disable this bool afterwards!")]
        [SerializeField] private bool updateInRealtime;

        [SerializeField] private bool disappearAfterUse = true;
        
        // PRIVATE VARIABLES
        
        private bool _pickedUp;
        protected Vector3 originalScale;
        private Collider _collider;
        
        private InputSystem_Actions _inputSystem;
        private InputAction _interact;

        protected ItemPickup player;

        private float _dropTimer;

        private bool isClose;
        private bool wasCloseLastFrame;
        
        // PUBLIC FUNCTIONS

        public abstract void OnPickup(ItemPickup player);
        public abstract bool ActivateCheck(ItemPickup player);

        public abstract void Activate(ItemPickup player);

        protected abstract void OnProximityEnter();
        protected abstract void OnProximityStay();
        protected abstract void OnProximityExit();

        public bool DropCheck()
        {
            if (_dropTimer <= 0)
            {
                StartCoroutine(Drop());
                return true;
            }

            return false;
        }

        public IEnumerator Drop()
        {
            if (!canBeDropped) yield break;

            transform.parent = null;
            _collider.enabled = true;
            _collider.isTrigger = false;
            if (TryGetComponent(out Rigidbody rb)) rb.isKinematic = false;

            transform.localScale = originalScale;
            
            yield return new WaitForSeconds(pickupCooldown);
            _collider.isTrigger = true;
            if (TryGetComponent(out Rigidbody rb2)) rb2.isKinematic = true;

            _pickedUp = false;
        }
        
        // PRIVATE FUNCTIONS

        private void OnEnable()
        {
            _inputSystem = new InputSystem_Actions();
            
            _interact = _inputSystem.Player.Interact;
            _interact.Enable();
            _interact.started += Interact;
            
            originalScale = transform.localScale;
            _collider = GetComponent<Collider>();

        }

        private void Start()
        {
            player = Player.Player.Instance.GetComponent<ItemPickup>();
        }

        private void OnDisable()
        {
            _interact.Disable();
        }

        private void Interact(InputAction.CallbackContext context)
        {
            PickUp(player.transform);
        }

        private void PickUp(Transform playerTransform)
        {
            if (_pickedUp || !isClose) return;
            
            if (activateInstantly)
            {
                _pickedUp = true;
                Activate(player);
                gameObject.SetActive(false);
                return;
            }

            if (!player.ItemPickUp(this)) return;

            _dropTimer = 1;
            _pickedUp = true;
            _collider.enabled = false;
            if (TryGetComponent(out Rigidbody rb)) rb.isKinematic = true;
            
            player.PositionItem(positionOffset, rotation, originalScale, scale);
            
            OnPickup(player);
        }
        private void Update()
        {
            if (player == null)
            {
                player = Player.Player.Instance.GetComponent<ItemPickup>();
                if (player == null) return;
            }
            isClose = !_pickedUp && Vector3.Distance(player.transform.position, transform.position) < 3 &&
                      player.ItemInMouth == null;
            
            if (updateInRealtime && _pickedUp)
            {
                player.PositionItem(positionOffset, rotation, originalScale, scale);
            }

            if (isClose)
            {
                if (!wasCloseLastFrame)
                {
                    print(wasCloseLastFrame);
                    OnProximityEnter();
                }
                
                OnProximityStay();
            }
            else if (wasCloseLastFrame)
            {
                OnProximityExit();
            }

            if (_dropTimer > 0)
            {
                _dropTimer -= Time.deltaTime;
            }
            
            
            wasCloseLastFrame = isClose;
        }

        public void Disable()
        {
            if (disappearAfterUse)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
