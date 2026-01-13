using General.Player;
using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace General.Interactable
{
    public class Campfire : Player.Interactable
    {
        [SerializeField] private bool litByDefault = false;
        [Space]
        [SerializeField] private Transform canvasGamepad;
        [SerializeField] private Transform canvasKeyboard;
        [SerializeField] private Transform canvasRemaining;
        [SerializeField] private Transform canvasMissingOrbs;
        [SerializeField] private Transform canvasTimer;
        [SerializeField] private ParticleSystem unlitParticleSystem;
        [Range(1, 10)][SerializeField] private int orbsRequired = 1;
        private bool _animationInProgress = false;
        private TextMeshProUGUI _remainingText;
        private int _orbsEntered;
        private Transform _currentCanvas; 
        [SerializeField] private GameObject fire;
        [SerializeField] private float orbYOffset;

        public float duration = 60.0f;
        public float CurrentDuration { get; private set; }
        private Image[] _timer;
        public int Index { get; private set; }
        private PlayerController _player;

        [HideInInspector] public bool isLit;

        private Transform _camera;
        private FollowingOrbs _playerOrbs;

        [SerializeField] private AnimationCurve turnOnCurve;

        [SerializeField] private Light spotlight;
        private float intensity;
        private float range;

        [Space] 
        [Header("Healing settings")] 
        [SerializeField] private int healHP = 1;
        [Tooltip("Seconds between heals")]
        [SerializeField] private float healCooldown = 1;
        [Tooltip("Distance to fire to regenerate")]
        [SerializeField] private float healRange = 5;

        private float healTimer;

        private void Start()
        {
            _camera = Camera.main.transform;
            _playerOrbs = General.Player.Player.Instance.GetComponent<FollowingOrbs>();
            spotlight.enabled = false;
            _timer = canvasTimer.GetComponentsInChildren<Image>();
            _player = Player.Player.Instance.PlayerController;

            intensity = spotlight.intensity;
            range = spotlight.range;
            
            unlitParticleSystem.Play();

            Campfire[] allCampfires = FindObjectsByType<Campfire>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
            
            for (int i = 0; i < allCampfires.Length; i++)
            {
                if (allCampfires[i] == this)
                {
                    Index = i;

                    i = allCampfires.Length;
                }
            }
            
            if (SaveSystem.save != null && 
                SaveSystem.save.campfireDurations != null &&
                SaveSystem.save.campfireDurations.Length >= allCampfires.Length)
            {
                CurrentDuration = SaveSystem.save.campfireDurations[Index];
                if (CurrentDuration > 0)
                {
                    litByDefault = true;
                }
            }

            if (litByDefault)
            {
                Light();
                interacted = true;
                spotlight.enabled = true;
            }
            else
            {
                CurrentDuration = 0;
                
                if (orbsRequired > 1)
                {
                    AutoSetInteracted = false;
                    _remainingText = canvasRemaining.GetComponentInChildren<TextMeshProUGUI>();
                }
            }
        }

        private void Update()
        {
            CanInteract = _playerOrbs.OrbCount != 0 && !_animationInProgress;
            
            // if in range and cannot interact with campfire...
            if (!interacted && !CanInteract && Vector3.Distance(_playerOrbs.transform.position, transform.position) < 5)
            {
                canvasMissingOrbs.gameObject.SetActive(true);
                canvasMissingOrbs.LookAt(_camera);
                canvasMissingOrbs.localRotation *= Quaternion.Euler(0, 180, 0);
            }
            else
            {
                canvasMissingOrbs.gameObject.SetActive(false);
            }

            if (!isLit && orbsRequired > 1 && Vector3.Distance(_playerOrbs.transform.position, transform.position) < 5)
            {
                canvasRemaining.gameObject.SetActive(true);
                canvasRemaining.LookAt(_camera);
                canvasRemaining.localRotation *= Quaternion.Euler(0, 180, 0);
                _remainingText.text = _orbsEntered + " / " + orbsRequired;
                canvasMissingOrbs.gameObject.SetActive(false);
            }
            else
            {
                canvasRemaining.gameObject.SetActive(false);
            }
            
            if (!isLit)
            {
                return;
            }

            if(CurrentDuration <= 0) UnLight();
            else if (!float.IsPositiveInfinity(duration))
            {
                CurrentDuration -= Time.deltaTime;
                
                canvasTimer.gameObject.SetActive(true);
                foreach (var fillable in _timer)
                {
                    fillable.fillAmount = CurrentDuration / duration;
                }
                canvasTimer.LookAt(_camera);
                canvasTimer.localRotation *= Quaternion.Euler(0, 180, 0);

                if (Vector3.Distance(_player.transform.position, transform.position) < healRange)
                {
                    healTimer -= Time.deltaTime;

                    if (healTimer <= 0)
                    {
                        Entity playerEntity = _player.GetComponent<Entity>();
                        
                        playerEntity.health += healHP;
                        if (playerEntity.health > playerEntity.maxHealth)
                            playerEntity.health = playerEntity.maxHealth;
                        healTimer = healCooldown;
                    }
                }
            }
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
            _currentCanvas.LookAt(_camera);
            _currentCanvas.localRotation *= Quaternion.Euler(0, 180, 0);
        }

        public override void PlayerProximityExit()
        {
            _currentCanvas.gameObject.SetActive(false);
        }

        public override void Interact()
        {
            _animationInProgress = true;
            _playerOrbs.ConsumeOrb(transform.position + Vector3.up * orbYOffset, 1, Light);
        }

        private void Light()
        {
            _orbsEntered++;
            _animationInProgress = false;
            unlitParticleSystem.Stop();
            
            if (_orbsEntered != orbsRequired) return;
            
            CurrentDuration = duration;
            isLit = true;
            fire.SetActive(true);
            spotlight.enabled = true;
            interacted = true;
            _ = TurnOn();
        }
        private async Awaitable TurnOn()
        {
            for(float i = 0; i < 1; i += Time.deltaTime)
            {
                Fade(turnOnCurve.Evaluate(i));
                await Awaitable.NextFrameAsync();
            }
            Fade(1);
        }
        private void UnLight()
        {
            isLit = false;
            fire.SetActive(false);
            interacted = false;
            spotlight.enabled = false;
            _orbsEntered = 0;
            unlitParticleSystem.Play();
        }

        private void Fade(float val)
        {
            fire.transform.localScale = new Vector3(val, val, val);
            spotlight.intensity = Mathf.Lerp(0, intensity, val);
            spotlight.range = Mathf.Lerp(0, range , val);
        }
    }
}
