using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace General.Player
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(CharacterController))]
    public class LegacyMovement : MonoBehaviour
    {
        //PUBLIC VARIABLES
        
        public float PlayerStamina { get; private set; }
        [Header("If you're confused about anything, hover the mouse over the variable,\n" +
                "some of them have helpful tooltips.\n" +
                "If you're still confused, ask me (Anton Lanning) for help!")]
        
        // PRIVATE VARIABLES THAT CAN BE CHANGED IN THE INSPECTOR
            
        [Header("Change these if you want")]
        [SerializeField] private float playerSpeed = 5;
        [SerializeField] private float sprintSpeed = 8;
        [SerializeField] private float rotationSpeed = 10;
        [SerializeField] private float jumpPower = 1;
        [Tooltip("Measured in seconds")] [SerializeField] private float jumpTime = 0.5f;
        [Space]
        [Tooltip("Measured in seconds")] [SerializeField] private float staminaDropSpeed = 3;
        [Tooltip("Measured in seconds")] [SerializeField] private float staminaHealSpeed = 5;
        [Space]
        [Tooltip("Check this if you want to disable the player to point in the velocity direction during a jump. " +
                 "This looks good when the player jumps while moving, but is a disappointment when the player jumps without moving. " +
                 "I am open for suggestions on improvement for this, but I think the animators will do a better job.")] 
        [SerializeField] private bool rotatePlayerWhenJumping = false;

        // DEBUG VARIABLES 
        
        [Header("FOR DEBUGGING PURPOSES!")]
        [SerializeField] private TextMeshProUGUI staminaText;
        private bool _staminaText;
        [SerializeField] private Text staminaLegacyText;
        private bool _staminaLegacyText;
        [Space]
        [SerializeField] private TextMeshProUGUI groundedText;
        private bool _groundedText;
        [SerializeField] private Text groundedLegacyText;
        private bool _groundedLegacyText;
        [Space]
        [SerializeField] private TextMeshProUGUI jumpingText;
        private bool _jumpingText;
        [SerializeField] private Text jumpingLegacyText;
        private bool _jumpingLegacyText;
        
        Vector3 _moveDirection;
        
        [Space] 
        
        // PRIVATE VARIABLES - in no particular order. Good luck :)
        
        private InputSystem_Actions _playerControls;
        private InputAction _move;
        private Vector2 _moveDirectionV2;
        private Vector3 _gravityVelocity;
        private Quaternion _targetRotation;
        private Quaternion _startRotation;
        private CharacterController _characterController;
        private float _rotationT;
        private float _rotationDiff;
        private Camera _mainCamera;
        private Vector3 _lastPosition;
        private bool _wasGroundedLastFrame;
        private InputAction _sprint;
        private bool _sprinting = false;
        private InputAction _jump;
        private bool _jumping;
        private Item.Item _itemInMouth;

        // PUBLIC FUNCTIONS
        
        public bool ItemPickUp(Item.Item item)
        {
            if (_itemInMouth != null) return false;

            _itemInMouth = item;
            
            return true;
        }
        
        // PRIVATE FUNCTIONS
        
        private void Awake()
        {
            _playerControls = new InputSystem_Actions();
        }
    
        // Initialization
        private void OnEnable()
        {
            _move = _playerControls.Player.Move;
            _move.Enable();

            _sprint = _playerControls.Player.Sprint;
            _sprint.Enable();
            _sprint.performed += OnSprintStart;
            _sprint.canceled += OnSprintExit;

            _jump = _playerControls.Player.Jump;
            _jump.Enable();
            _jump.performed += Jump;
            
            _characterController = GetComponent<CharacterController>();
            _mainCamera = Camera.main;
            
            PlayerStamina = 1;
            
            //Debug initialization

            _staminaText = staminaText != null;
            _staminaLegacyText = staminaLegacyText != null;
            
            _groundedText = groundedText != null;
            _groundedLegacyText = groundedLegacyText != null;
            
            _jumpingText = jumpingText != null;
            _jumpingLegacyText = jumpingLegacyText != null;
        }

        private void OnDisable()
        {
            _move.Disable();
            _sprint.Disable();
            _jump.Disable();
        }

        private void Update()
        {
            _moveDirectionV2 = _move.ReadValue<Vector2>();
            
            if (_characterController.isGrounded)
            {
                _gravityVelocity = Vector3.zero;
            }
            
            
            if (_jumping)
            {
                _gravityVelocity = Vector3.zero;
            }
            else
            {
                _gravityVelocity += Physics.gravity * Time.deltaTime;
            }
            
            if (_sprinting && PlayerStamina > 0) // Movement of player
            {
                _moveDirection = (_moveDirectionV2.x * ToPivotRotation(_mainCamera.transform.right) + _moveDirectionV2.y * ToPivotRotation(_mainCamera.transform.forward))
                    * sprintSpeed;
                
                PlayerStamina -= Time.deltaTime / staminaDropSpeed;
            }
            else
            {
                _moveDirection = (_moveDirectionV2.x * ToPivotRotation(_mainCamera.transform.right) + _moveDirectionV2.y * ToPivotRotation(_mainCamera.transform.forward))
                    * playerSpeed;
                
                if (_sprinting) PlayerStamina -= Time.deltaTime / staminaDropSpeed;
                else PlayerStamina += Time.deltaTime / staminaHealSpeed;
                
            }

            _moveDirection += _gravityVelocity;
            
            PlayerStamina = Mathf.Clamp01(PlayerStamina);
            
            if (!_jumping)
                _characterController.Move(_moveDirection * Time.deltaTime);

            if (GetMoveVector() != Vector3.zero) // Visual rotation of player
            {
                if (_targetRotation !=
                    Quaternion.LookRotation(GetMoveVector(), Vector3.up))
                {
                    _rotationT = 0;
                    _startRotation = transform.rotation;
                    
                    if (!rotatePlayerWhenJumping && (!_characterController.isGrounded || !_wasGroundedLastFrame))
                    {
                        if (ToPivotRotation(GetMoveVector()) != Vector3.zero)
                        {
                            _targetRotation = Quaternion.LookRotation(ToPivotRotation(GetMoveVector()), Vector3.up);
                        }
                    }
                    else
                    {
                        _targetRotation = Quaternion.LookRotation(GetMoveVector(), Vector3.up);
                    }
                    
                    _rotationDiff = Quaternion.Angle(_startRotation, _targetRotation);
                }
                
                _rotationT += Mathf.Clamp(Time.deltaTime * rotationSpeed * 60 / _rotationDiff, 0.05f, 1);
            }
            
            
            
            transform.rotation = Quaternion.Slerp(_startRotation, _targetRotation, _rotationT);

            _lastPosition = transform.position;

            if (_staminaText) 
                staminaText.text = "STAMINA: " + (Mathf.Round(PlayerStamina * 100) / 100).ToString(CultureInfo.CurrentCulture);
            if (_staminaLegacyText) 
                staminaLegacyText.text = "STAMINA: " + (Mathf.Round(PlayerStamina * 100) / 100).ToString(CultureInfo.CurrentCulture);
            
            if (_groundedText) 
                groundedText.text = "GROUNDED: " + (_characterController.isGrounded ? "TRUE" : "FALSE");
            if (_groundedLegacyText) 
                groundedLegacyText.text = "GROUNDED: " + (_characterController.isGrounded ? "TRUE" : "FALSE");
            
            if (_jumpingText) 
                jumpingText.text = "JUMPING: " + (_jumping ? "TRUE" : "FALSE");
            if (_jumpingLegacyText) 
                jumpingLegacyText.text = "JUMPING: " + (_jumping ? "TRUE" : "FALSE");

            _wasGroundedLastFrame = _characterController.isGrounded;
        }

        private Vector3 GetMoveVector()
        {
            return (transform.position - _lastPosition).normalized;
        }

        private void OnSprintStart(InputAction.CallbackContext context)
        {
            _sprinting = true;
        }
        
        private void OnSprintExit(InputAction.CallbackContext context)
        {
            _sprinting = false;
        }

        private static Vector3 ToPivotRotation(Vector3 vector)
        {
            return new Vector3(vector.x, 0, vector.z).normalized;
        }

        private void Jump(InputAction.CallbackContext context)
        {
            StartCoroutine(PerformJump());
        }

        private IEnumerator PerformJump()
        {
            if (!_characterController.isGrounded || _jumping) yield break;
            
            _jumping = true;
            
            for (float i = -1; i < 0; i += Time.deltaTime / jumpTime)
            {
                _characterController.Move(Time.deltaTime * jumpPower / 4 * -Physics.gravity + _moveDirection * Time.deltaTime);
                yield return null;
                if (_characterController.isGrounded) break;
            }

            _jumping = false;
        }
    }
}

