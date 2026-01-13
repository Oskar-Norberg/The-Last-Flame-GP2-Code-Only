using UnityEngine;
using General.Player;
using UnityEngine.UI;

namespace General.Interactable
{
    public class Torch : Player.Interactable
    {
        
        [SerializeField] private bool litByDefault = false;
        [Space]
        [SerializeField] private Transform canvasGamepad;
        [SerializeField] private Transform canvasKeyboard;
        [SerializeField] private Transform canvasMissingOrbs;
        [SerializeField] private Transform canvasTimer;
        private Transform _currentCanvas; 
        [SerializeField] private GameObject fire;
        [SerializeField] private bool stationary = false;
        [SerializeField] private Light spotLight;

        public float duration = 60.0f;
        
        public float CurrentDuration { get; private set; }
        private Image[] _timer;
        public int Index { get; private set; }
        
        private PlayerController _player;

        public bool isGathered { get; set; }
        public bool isLit { get; private set; }

        private Transform _camera;
        private FollowingOrbs _playerOrbs;
        
        private void Start()
        {
            _camera = Camera.main.transform;
            _playerOrbs = General.Player.Player.Instance.GetComponent<FollowingOrbs>();
            isGathered = stationary;
            _timer = canvasTimer.GetComponentsInChildren<Image>();
            
            Torch[] allTorches = FindObjectsByType<Torch>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);

            for (int i = 0; i < allTorches.Length; i++)
            {
                if (allTorches[i] == this)
                {
                    Index = i;

                    i = allTorches.Length;
                }
            }
            
            if (SaveSystem.save != null && 
                SaveSystem.save.torchDurations != null &&
                SaveSystem.save.torchDurations.Length >= allTorches.Length)
            {
                CurrentDuration = SaveSystem.save.torchDurations[Index];
                if (CurrentDuration > 0)
                {
                    litByDefault = true;
                }
            }
            
            if (litByDefault)
            {
                Light();
                interacted = true;
                spotLight.enabled = true;
            }
            else
            {
                spotLight.enabled = false;
                CurrentDuration = 0;
            }
        }

        private void Update()
        {
            CanInteract = _playerOrbs.OrbCount != 0 && isGathered;

            // if in range and cannot interact with campfire...
            if (isGathered && !interacted && !CanInteract && Vector3.Distance(_playerOrbs.transform.position, transform.position) < 5)
            {
                canvasMissingOrbs.gameObject.SetActive(true);
                canvasMissingOrbs.LookAt(_camera);
                canvasMissingOrbs.localRotation *= Quaternion.Euler(0, 180, 0);
            }
            else
            {
                canvasMissingOrbs.gameObject.SetActive(false);
            }
            
            
            if (interacted && Vector3.Distance(_playerOrbs.transform.position, transform.position) < 8)
            {
                
                
            }
            else
            {
            }
            
            if(!isLit) return;

            if(CurrentDuration <= 0) UnLight();
            else
            {
                CurrentDuration -= Time.deltaTime;
                
                canvasTimer.gameObject.SetActive(true);
                foreach (var fillable in _timer)
                {
                    fillable.fillAmount = CurrentDuration / duration;
                }
                canvasTimer.LookAt(_camera);
                canvasTimer.localRotation *= Quaternion.Euler(0, 180, 0);
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
            Light();
            //_playerOrbs.ConsumeOrb(transform.position, 1, Light);
        }

        private void Light()
        {
            CurrentDuration = duration;
            isLit = true;
            fire.SetActive(true);
            spotLight.enabled = true;
        }
        private void UnLight()
        {
            canvasTimer.gameObject.SetActive(false);
            isLit = false;
            fire.SetActive(false);
            interacted = false;
            spotLight.enabled = false;
        }
    }
}
