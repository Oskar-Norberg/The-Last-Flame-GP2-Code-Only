using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace General.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Animator))]
    public class PlayerController : MonoBehaviour, IUpdateable
    {
        private static readonly int Movement = Animator.StringToHash("Movement");
        public float CurrentPlayerSpeed { get; private set; }
        public enum Controller {Keyboard, Gamepad}
        public Controller LastInput { get; private set; }
        
        [SerializeField] private float speed = 5.0f;
        [SerializeField] private float sprintSpeed = 10.0f;
        [SerializeField] private float rotationSpeed = 5.0f;
        [SerializeField] private float secondsToFinalSpeed = 0.1f;
        [Space]
        [SerializeField] private float groundCheckRange = 0.5f;
        [SerializeField] private float slopeLimit = 45f;
        [Space]
        [SerializeField] private float dashTimer = 0.2f;
        [SerializeField] private float dashSpeed = 20.0f;
        [SerializeField] private float dashCoolDownTimer = 0.5f;
        [Space]
        [SerializeField] private float gravity = -9.81f;
        [Space]
        [SerializeField] private LayerMask groundLayer;
        [Space]
        [SerializeField] private float jumpTime = 0.2f;
        [SerializeField] private float jumpHeight = 3;
        [SerializeField] private float jumpFloatTime = 0;
        [SerializeField] private float jumpCooldown = 2;
        [Space]
        [SerializeField] private Image blackPanel;
        [SerializeField] private Image frostPanel;
        private CharacterController _characterController;
        
        private Vector3 _groundNormal = Vector3.up;
        private Vector3 _directionInput;
        private bool _jump;
        private bool _dash;
        private bool _sprint;
        private bool _interact;
        private bool _carrying;
        private bool _drop;
        public bool _canSprint;
        private Vector3 _targetXZDirection;
        private Transform _cameraTrans;
        private Vector3 _turnDirection;
        private float _currentJumpForce;
        private float _dashTimer;
        private Vector3 _downVector = Vector3.down;
        private bool _isJumping;
        private Coroutine _jumpProcess;
        private float _timeSinceInput;
        private float _originalWalkingSpeed;
        private Vector3 _originalVector;
        private Animator _animator;
        private float _movementFloat;
        private bool _isSprinting;
        private float _jumpTimer;
        private PlayerEntity _entity;

        public UnityEvent onJumped;
        
        public UnityEvent<bool> onGroundedStatusChanged;

        public UnityEvent<bool> onSprintStatusChanged;

        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
            _cameraTrans = Camera.main.transform;
            _animator = GetComponent<Animator>();
            _animator.SetBool("isMoving", true);
            _entity = GetComponent<PlayerEntity>();
            if (blackPanel == null)
            {
                blackPanel = GameObject.Find("DeathBlackOverlay").GetComponent<Image>();
            }
            
            if (SceneManager.GetActiveScene().buildIndex == 1 && SaveSystem.save.current_checkpoint == 0)
                _canSprint = false;
            else 
                _canSprint = true;
        }

        public void CustomFixedUpdate(float fixedDeltaTime)
        {
        }

        public void CustomUpdate(float deltaTime)
        {
            Direction();
            
            DirectionalMovement();
            
            RotationalMovement();

            Jump();

            Animate();
            
            CheckController();

            UpdateBlackPanel();

        }

        private void CheckController()
        {
            if (Gamepad.current == null)
            {
                LastInput = Controller.Keyboard;
                return;
            }
            
            if (Gamepad.current.IsActuated(0))
            {
                LastInput = Controller.Gamepad;
            }
            else if (Keyboard.current.IsActuated(0))
            {
                LastInput = Controller.Keyboard;
            }
        }

        private void Direction()
        {
            Vector3 forward = new Vector3(_cameraTrans.forward.x, 0, _cameraTrans.forward.z).normalized;
            Vector3 right = new Vector3(_cameraTrans.right.x, 0, _cameraTrans.right.z).normalized;

            _targetXZDirection = (forward * _directionInput.z + right * _directionInput.x).normalized;

            _directionInput = _directionInput.magnitude > 1 ? _directionInput.normalized : _directionInput;
            
            if (_originalWalkingSpeed < _directionInput.magnitude - 0.01f || 
                _originalWalkingSpeed > _directionInput.magnitude + 0.01f )
            {
                _originalVector = transform.forward * (_timeSinceInput * _originalWalkingSpeed);
                _timeSinceInput = 0;
            }
            
            
            _originalWalkingSpeed = _directionInput.magnitude;
        }

        private void DirectionalMovement()
        {
            _timeSinceInput += Time.deltaTime / secondsToFinalSpeed;
            _timeSinceInput = Mathf.Clamp01(_timeSinceInput);
            float multiplier;
            if (_sprint && _canSprint)
            {
                multiplier = sprintSpeed;
                _isSprinting = true;
            }
            else
            {
                multiplier = speed;
                _isSprinting = false;
            }
            
            
            CurrentPlayerSpeed = multiplier;

            if (!_characterController.isGrounded && !_isJumping) _downVector = Vector3.down;
            
            Vector3 wishedDirection = Vector3.Lerp(_originalVector, _targetXZDirection * _directionInput.magnitude, _timeSinceInput) 
                                      * (Time.deltaTime * multiplier);
            Vector3 gravityVector = _downVector * (gravity * Time.deltaTime);
            
            wishedDirection -= gravityVector;

            _characterController.Move(wishedDirection);
        }

        private void Animate()
        {
            //_animator.speed = _targetXZDirection.magnitude > 0 ? _targetXZDirection.magnitude : 1;
            
            if (_targetXZDirection.magnitude > 0)
            {
                SetMovementFloat(_isSprinting ? 2 : 1, secondsToFinalSpeed);
            }
            else
            {
                SetMovementFloat(0, secondsToFinalSpeed);
            }
            
            _animator.SetFloat("Movement", _movementFloat);
        }

        private void SetMovementFloat(float value, float seconds)
        {
            if (value > _movementFloat)
            {
                _movementFloat += Time.deltaTime / seconds;
                if (value < _movementFloat)
                    _movementFloat = value;
            }
            else if (value < _movementFloat)
            {
                _movementFloat -= Time.deltaTime / seconds;
                if (value > _movementFloat)
                    _movementFloat = value;
            }
        }
        
        private void RotationalMovement()
        {
            if ((_targetXZDirection.magnitude == 0.00f)) return;
            
            RaycastForTerrainSlope();
            Quaternion targetRotation = Quaternion.LookRotation(_turnDirection, Vector3.up);
            transform.rotation =
                Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        private void RaycastForTerrainSlope() //For Rotation V2
        {
            // voodoo magic
            Vector3 offsetForward = transform.forward * 0.1f;
            Vector3 offsetBackward = transform.forward * -0.1f;
            Vector3 upwardOffset = Vector3.up * 0.1f;

            Vector3 startForward = transform.position + offsetForward + upwardOffset;
            Vector3 startBackward = transform.position + offsetBackward + upwardOffset;

            if (Physics.Raycast(startForward, Vector3.down, out RaycastHit hit1, groundCheckRange, groundLayer) &&
                Physics.Raycast(startBackward, Vector3.down, out RaycastHit hit2, groundCheckRange, groundLayer))
            {
                Vector3 heightDifference = hit1.point - hit2.point;
                _turnDirection = new Vector3(_targetXZDirection.x, heightDifference.normalized.y, _targetXZDirection.z).normalized;
            }
            else
            {
                _turnDirection = _targetXZDirection;
            }
        }

        // SLIDE RELATED
        private void OnControllerColliderHit (ControllerColliderHit hit)
        {
            if (_isJumping) return;
            
            if (Vector3.Angle(hit.normal, Vector3.up) < slopeLimit)
            {
                _downVector = Vector3.down;
                
                return;
            }

            _downVector = hit.normal + Vector3.down * (2 * Mathf.Sqrt(0.5f));
        }
        
        private void Jump()
        {
            if (_jumpTimer > 0 && !_isJumping) _jumpTimer -= Time.deltaTime;
            
            if (_jump && _characterController.isGrounded && !_isJumping && _downVector == Vector3.down && _jumpTimer <= 0)
            {
                onJumped?.Invoke();
                _jumpProcess = StartCoroutine(JumpProcess());
            }
            
            else if ((_characterController.collisionFlags & CollisionFlags.Above) != 0 ||
                (_characterController.collisionFlags & CollisionFlags.Below) != 0)
            {
                if (_jumpProcess == null) return;
                StopCoroutine(_jumpProcess);
                _isJumping = false;
                _animator.SetBool("inAir", false);
                onGroundedStatusChanged?.Invoke(true);
            }
        }

        void UpdateBlackPanel()
        {
            if (_entity.health <= 20)
            {
                blackPanel.color = new Color(1, 1, 1, 1 - _entity.health / 20f);
            }
        }

        private IEnumerator JumpProcess()
        {
            _jumpTimer = jumpCooldown;
            
            _animator.SetTrigger("Jump");
            GameManager gameManager = GameManager.Instance;
            
            onGroundedStatusChanged?.Invoke(false);

            for (float i = 0; i < Math.PI; i += (Time.deltaTime / jumpTime) * (gameManager.isPaused ? 0 : 1))
            {
                _downVector = Vector3.up * (Mathf.Cos(i) * jumpHeight);
                yield return null;
                _isJumping = true;
                _animator.SetBool("inAir", true);
            }

            _isJumping = false;
            _downVector = Vector3.down;
            
            onGroundedStatusChanged?.Invoke(true);
        }
        
        
        #region Input Mapping
        public void MovementInput(InputAction.CallbackContext context)
        {
            Vector2 read = context.ReadValue<Vector2>();
            _directionInput.x = read.x;
            _directionInput.z = read.y;
        }

        public void JumpInput(InputAction.CallbackContext context)
        {
            _jump = context.performed;
        }

        public void DashInput(InputAction.CallbackContext context)
        {
            _dash = context.performed;
        }

        public void SprintInput(InputAction.CallbackContext context)
        {
            _sprint = context.performed;
            
            // Oskar Jackson changes
            if (context.performed)
            {
                onSprintStatusChanged?.Invoke(true);
            }
            else if (context.canceled)
            {
                onSprintStatusChanged?.Invoke(false);
            }
        }

        public void InteractionInput(InputAction.CallbackContext context)
        {
            _interact = context.performed;
        }

        public void DropInput(InputAction.CallbackContext context)
        {
            _drop = context.performed;
        }
        #endregion
    }
}