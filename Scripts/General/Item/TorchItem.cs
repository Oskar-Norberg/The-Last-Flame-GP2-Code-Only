using System;
using General.Interactable;
using General.Player;
using UnityEngine;

namespace General.Item
{
    [RequireComponent(typeof(Torch))]
    [RequireComponent(typeof(Rigidbody))]
    public class TorchItem : Item
    {
        [Header("Torch settings. ESSENTIAL TO SET UP!!!!")]
        [SerializeField] private Transform targetQP;
        [SerializeField] private float distanceToTarget = 5;

        [Header("Carry radius is a radius around the target. If the player exits the radius\n" +
                "then the item will be dropped.\n" +
                "Turning on Automatic Carry Radius Detection means that it makes a guess\n" +
                "on how big the value should be based on the initial distance before picked up.")]
        [SerializeField] private bool automaticCarryRadiusDetection = true;
        [SerializeField] private float carryRadius = 50;

        [Space] 
        [SerializeField] private Transform canvasForGamepadInput;
        [SerializeField] private Transform canvasForKeyboardInput;
        private Transform _currentInputCanvas;

        private Torch _interactable;
        private Rigidbody _rigidbody;
        private ItemPickup currentPlayer;
        private Transform _camera;
        
        private void Awake()
        {
            _interactable = GetComponent<Torch>();
            _interactable.isGathered = false;
            _rigidbody = GetComponent<Rigidbody>();

            if (automaticCarryRadiusDetection)
            {
                carryRadius = Vector3.Distance(transform.position, targetQP.position) + 5;
            }
        }

        private void Start()
        {
            _camera = Camera.main.transform;
        }

        public override void OnPickup(ItemPickup player)
        {
            currentPlayer = player;
        }

        public override bool ActivateCheck(ItemPickup player)
        {
            return Vector3.Distance(transform.position, targetQP.position) < distanceToTarget;
        }

        public override void Activate(ItemPickup player)
        {
            _rigidbody.isKinematic = true;
            transform.rotation = Quaternion.identity;
            transform.position += Vector3.down * 0.5f;
            transform.localScale = originalScale;
            _interactable.isGathered = true;
        }

        protected override void OnProximityEnter()
        {
            switch (General.Player.Player.Instance.PlayerController.LastInput)
            {
                case PlayerController.Controller.Gamepad:
                    _currentInputCanvas = canvasForGamepadInput;
                    break;
                case PlayerController.Controller.Keyboard:
                    _currentInputCanvas = canvasForKeyboardInput;
                    break;
            }

            _currentInputCanvas.gameObject.SetActive(true);
        }

        protected override void OnProximityStay()
        {
            _currentInputCanvas.LookAt(_camera); 
            _currentInputCanvas.localRotation *= Quaternion.Euler(0, 180, 0);
        }

        protected override void OnProximityExit()
        {
            _currentInputCanvas.gameObject.SetActive(false);
        }

        private void LateUpdate()
        {
            if (Vector3.Distance(transform.position, targetQP.position) > carryRadius)
            {
                if (currentPlayer == null)
                {
                    automaticCarryRadiusDetection = true;
                    carryRadius = Vector3.Distance(transform.position, targetQP.position) + 5;
                    Debug.LogWarning("Carry radius is lower than the distance to the target. " +
                                     "Automatic carry radius detector has been turned on.");
                }
                else
                {
                    currentPlayer.Drop();
                    currentPlayer = null;
                }
            }
        }
    }
}
