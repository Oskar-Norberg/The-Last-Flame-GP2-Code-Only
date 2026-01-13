using System;
using General.Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace General.Interactable
{
    public class GlowberryBush : Player.Interactable
    {
        [SerializeField] private Transform canvasGamepad;
        [SerializeField] private Transform canvasKeyboard;
        [SerializeField] private Transform canvasTimer;
        [SerializeField] private Transform canvasTooManyOrbs;
        private Transform _currentCanvas; 
        [SerializeField] private GameObject berries;
        [Header("Regeneration only happens if a fire, torch or QP is activated nearby")]
        [SerializeField] private float regenerationTime = 30;
        [Space]
        [SerializeField] private Campfire[] nearbyCampfires;
        [SerializeField] private Torch[] nearbyTorches;
        [SerializeField] private CheckPoint[] nearbyQuestPoints;

        private Transform _camera;
        private FollowingOrbs _playerOrbs;
        public float TimeSincePick { get; private set; }
        public int Index { get; private set; }
        private Image[] _timer;
        private bool _hasBerries = true;

        [SerializeField] private UnityEvent onInteract;
        
        private void Start()
        {
            _camera = Camera.main.transform;
            _playerOrbs = General.Player.Player.Instance.GetComponent<FollowingOrbs>();
            _timer = canvasTimer.GetComponentsInChildren<Image>();
            
            GlowberryBush[] allBushes = FindObjectsByType<GlowberryBush>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);

            for (int i = 0; i < allBushes.Length; i++)
            {
                if (allBushes[i] == this)
                {
                    Index = i;

                    i = allBushes.Length;
                }
            }
            
            if (SaveSystem.save != null && 
                SaveSystem.save.berryDurations != null &&
                SaveSystem.save.berryDurations.Length >= allBushes.Length)
            {
                TimeSincePick = SaveSystem.save.berryDurations[Index];
                if (TimeSincePick < regenerationTime)
                {
                    _hasBerries = false;
                    berries.SetActive(false);
                    interacted = true;
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
            onInteract?.Invoke();
            berries.SetActive(false);
            _hasBerries = false;
            TimeSincePick = 0;
            _playerOrbs.AddOrb(transform.position);
        }

        private void Update()
        {
            
            if (interacted && FireNearby() && Vector3.Distance(_playerOrbs.transform.position, transform.position) < 8)
            {
                
                TimeSincePick += Time.deltaTime;
                
                canvasTimer.gameObject.SetActive(true);
                foreach (var fillable in _timer)
                {
                    fillable.fillAmount = TimeSincePick / regenerationTime;
                }
                canvasTimer.LookAt(_camera);
                canvasTimer.localRotation *= Quaternion.Euler(0, 180, 0);
                
                if (TimeSincePick > regenerationTime)
                {
                    berries.SetActive(true);
                    interacted = false;
                    _hasBerries = true;
                }
            }
            else
            {
                canvasTimer.gameObject.SetActive(false);
            }

            
            
            if (_hasBerries && !interacted && !CanInteract && Vector3.Distance(_playerOrbs.transform.position, transform.position) < 5)
            {
                canvasTooManyOrbs.gameObject.SetActive(true);
                canvasTooManyOrbs.LookAt(_camera);
                canvasTooManyOrbs.localRotation *= Quaternion.Euler(0, 180, 0);
            }
            else
            {
                canvasTooManyOrbs.gameObject.SetActive(false);
            }

            if (_hasBerries)
            {
                CanInteract = (_playerOrbs.OrbCount != _playerOrbs.MaximumOrbCount);
            }
            else
            {
                CanInteract = false;
            }
        }

        private bool FireNearby()
        {
            bool returnbool = false;

            foreach (var ls in nearbyCampfires)
            {
                returnbool = ls.isLit || returnbool;
            }
            
            foreach (var ls in nearbyTorches)
            {
                returnbool = ls.isLit || returnbool;
            }
            
            foreach (var ls in nearbyQuestPoints)
            {
                returnbool = ls.interacted || returnbool;
            }

            return returnbool;
        }
    }
}
