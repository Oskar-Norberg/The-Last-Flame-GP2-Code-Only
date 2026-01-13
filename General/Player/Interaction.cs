using UnityEngine;
using UnityEngine.InputSystem;

namespace General.Player
{
    [RequireComponent(typeof(ItemPickup))]
    public class Interaction : MonoBehaviour
    {
        [SerializeField] private bool useRaycast = true;
        [SerializeField] private float detectionRange = 5;
        private Interactable[] _interactables;
        private ItemPickup _itemPickup;
        private Interactable _highest = null;

        private InputSystem_Actions _inputSystem;
        private InputAction _interact;
        
        private void Awake()
        {
            _interactables = 
                FindObjectsByType<Interactable>(FindObjectsSortMode.None);

            _itemPickup = GetComponent<ItemPickup>();
        }

        private void OnEnable()
        {
            _inputSystem = new InputSystem_Actions();
            _interact = _inputSystem.Player.Interact;
            _interact.Enable();
            _interact.started += Interact;
        }

        private void OnDisable()
        {
            _interact.Disable();
        }

        private void Update()
        {
            Interactable oldHighest = _highest;
            _highest = null;
            
            int highestPrio = int.MinValue;
            
            foreach (Interactable interactable in _interactables)
            {
                if (interactable.interacted || !interactable.CanInteract) continue;
                
                if (Vector3.Distance(transform.position, interactable.transform.position) > detectionRange)
                    continue;

                if (_highest != null && _highest.priority < highestPrio)
                    continue;

                _highest = interactable;
                highestPrio = _highest.priority;
            }

            if (oldHighest != _highest)
            {
                _highest?.PlayerProximityEnter();
                oldHighest?.PlayerProximityExit();
            }
            
            _highest?.PlayerProximityStay();
        }

        private void Interact(InputAction.CallbackContext context)
        {
            if (_itemPickup.ItemInMouth != null) return;

            if (_highest == null) return;
            
            _highest.Interact();
            if (!_highest.AutoSetInteracted) return;
            _highest.interacted = true;
        }
    }

    public abstract class Interactable : MonoBehaviour
    {
        [Header("Leave this at false unless you have something else in mind.")]
        public bool interacted;

        public int priority = 1;

        public bool CanInteract { get; protected set; } = true;
        public bool AutoSetInteracted { get; protected set; } = true;
        public abstract void PlayerProximityEnter();
        public abstract void PlayerProximityStay();
        public abstract void PlayerProximityExit();
        public abstract void Interact();
    }
}
