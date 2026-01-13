using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using General.Player;

namespace General.Item
{
    public class Artifact : Player.Interactable
    {
        [Header("Activate instantly needs to be turned on in the inspector")]
        [SerializeField] private CinemachineCamera freelookCamera; 
        [SerializeField] private Camera mainCamera;
        [Space]
        [SerializeField] private Transform canvasGamepad;
        [SerializeField] private Transform canvasKeyboard;
        [SerializeField] private MenuManager menuManager;
        public bool PickedUp { get; private set; }
        public int index = 0;
        private Transform _currentCanvas; 
        private PlayerController _player;
        
        private Vector3 targetPosition;
        [SerializeField] private float movementSpeed = 5.0f;
        [SerializeField] private float smoothTime = 0.3f;
        
        [SerializeField] private float rotationSpeed = 5.0f;
        
        [SerializeField] private CodexNavigation codexNavigation;

        public Sprite artifactSprite;
        
        
        public delegate void OnArtifactPickedUpEventHandler(Artifact artifact);
        public static event OnArtifactPickedUpEventHandler OnArtifactPickedUp;

        private void OnEnable()
        {
            // targetPosition = new Vector3(mainCamera.gameObject.transform.position.x, mainCamera.gameObject.transform.position.y, mainCamera.gameObject.transform.position.z);
            if (freelookCamera == null)
                freelookCamera = FindAnyObjectByType<CinemachineOrbitalFollow>().GetComponent<CinemachineCamera>();
        }

        public override void PlayerProximityEnter()
        {
            switch (General.Player.Player.Instance.PlayerController.LastInput)
            {
                case PlayerController.Controller.Gamepad:
                    _currentCanvas = canvasGamepad;
                    break;
                case PlayerController.Controller.Keyboard:
                    _currentCanvas = canvasKeyboard;
                    break;
            }

            _currentCanvas.gameObject.SetActive(true);
        }

        public override void PlayerProximityStay()
        {
            _currentCanvas.LookAt(freelookCamera.transform);
            _currentCanvas.localRotation *= Quaternion.Euler(0, 180, 0);
        }

        public override void PlayerProximityExit()
        {
            _currentCanvas.gameObject.SetActive(false);
        }

        public override void Interact()
        {
            if (GameManager.Instance.currentStateType != typeof(PlayingState))
                return;
            
            PickedUp = true;
            CanInteract = false;
            gameObject.SetActive(false);
            
            // MoveArtifactToSpace();
            OnArtifactPickedUp?.Invoke(this);
            menuManager.ToggleCodex(true);
            codexNavigation.AddNewArtifact(index);
        }

        private void MoveArtifactToSpace()
        {
            gameObject.transform.SetParent(mainCamera.transform);
            // StartCoroutine(RotateContinuously());
        }
        
        private IEnumerator RotateContinuously()
        {
            while (true)
            {
                transform.Rotate(Vector3.up * rotationSpeed * Time.unscaledDeltaTime);
                yield return null;
            }
        }
    }
}
