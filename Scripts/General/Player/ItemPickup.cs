using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace General.Player
{
    public class ItemPickup : MonoBehaviour
    {
        // PUBLIC VARIABLES
        public Item.Item ItemInMouth { get; private set; }
        
        
        [FormerlySerializedAs("_itemInMouth")]
        [Header("If you're confused about anything, hover the mouse over the variable,\n" +
                "some of them have helpful tooltips.\n" +
                "If you're still confused, ask me (Anton Lanning) for help!")]
        [Space]
        
        // PRIVATE VARIABLES THAT CAN BE CHANGED IN THE INSPECTOR
        
        [Header("ESSENTIALS:")]
        [SerializeField] private Transform mouth;
        
        // PRIVATE VARIABLES
        
        private InputSystem_Actions _playerControls;
        private InputAction _activate;
        
    
        // PUBLIC FUNCTIONS
        
        public bool ItemPickUp(Item.Item item)
        {
            if (ItemInMouth != null) return false;
            
            ItemInMouth = item;
            ItemInMouth.transform.parent = mouth;
            
            return true;
        }

        public void PositionItem(Vector3 offset, Quaternion rotation, Vector3 originalScale, float scaleMultiplier)
        {
            ItemInMouth.transform.rotation = mouth.rotation;
            
            ItemInMouth.transform.localPosition = Vector3.zero + offset;
            ItemInMouth.transform.localRotation = rotation;
            ItemInMouth.transform.localScale = originalScale * scaleMultiplier / mouth.lossyScale.magnitude;
        }
        
        public void Drop()
        {
            if (ItemInMouth.DropCheck())
                ItemInMouth = null;
        }
        
        // PRIVATE FUNCTIONS
        private void OnEnable()
        { 
            _playerControls = new InputSystem_Actions();
            _activate = _playerControls.Player.Interact;
            _activate.Enable();
            _activate.started += ActivateItem;
        }

        private void OnDisable()
        {
            _activate.Disable();
        }

        private void ActivateItem(InputAction.CallbackContext context)
        {
            if (ItemInMouth == null) return;
            
            if (ItemInMouth.ActivateCheck(this))
            {
                ItemInMouth.transform.parent = null;
                ItemInMouth.Activate(this);
                ItemInMouth.Disable();
                ItemInMouth = null;
            }
            else
            {
                Drop();
            }
            
        }

        
    }
}

