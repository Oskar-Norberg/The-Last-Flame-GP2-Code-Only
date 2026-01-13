using System;
using System.Collections.Generic;
using General.Player;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CheckPoint : Interactable
{
    public Quest quest;
    private bool _questPointOn;
    private Transform _camera;
    private Transform _player;
    private List<GameObject> _canvasChildren = new List<GameObject>();

    public HumanRestSpace restSpace;

    public General.Interactable.Campfire[] campfires;
    public General.Interactable.Torch[] torches;
    public General.Item.Artifact[] artifacts;

    [Space] [SerializeField] private bool increaseOrbCap;

    [Space] [Tooltip("Please dont change this... ask milo if you want to know why.")]
    public int saveIndex;

    public GameObject lightUpArea;
    public TextMeshProUGUI conditionText;
    public Transform conditionCanvas;
    [SerializeField] private Transform interactCanvasKeyboard;
    [SerializeField] private Transform interactCanvasGamepad;
    private Transform currentCanvas;
    private CinemachineCamera freelookCamera;

    [SerializeField] private CinemachineCamera questCamera;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private GameObject campfireDirectionPrefabs;
    [SerializeField] private Material campfireDirectionMaterial;
    [SerializeField] private AnimationCurve directionCurve;
    [SerializeField] private float directionLength = 5;
    [SerializeField] private float directionDuration = 2;
    [SerializeField] private float heightOffset = 3;
    [Space] 
    [SerializeField] private GameObject conditionDisplay;
    [Space] 
    [SerializeField] private GameObject[] setActiveWhenInteracted;
    [SerializeField] private GameObject[] setInactiveWhenInteracted;
    private bool hintCooldown;

    public delegate void OnCheckpointLitEventHandler();

    public event OnCheckpointLitEventHandler OnCheckpointLit;

    private void Start()
    {
        _camera = Camera.main.transform;
        SaveSystem.Load();
        _questPointOn = SaveSystem.save.unlock_checkpoints[saveIndex];
        AutoSetInteracted = false;
        
        if (freelookCamera == null)
            freelookCamera = FindAnyObjectByType<CinemachineOrbitalFollow>().GetComponent<CinemachineCamera>();
        
        if (_questPointOn)
        {
            _ = LightQuestPoint(true);
            interacted = true;
        }

        _player = Player.Instance.transform;
    }


    public override void Interact()
    {
        if (!_questPointOn)
        {
            if (quest == null || quest.CanLight(this))
            {
                _questPointOn = true;
                interacted = false;
                
                foreach (var obj in setInactiveWhenInteracted)
                {
                    print("setting " + obj + " inactive");
                    obj.SetActive(false);
                }
                
                foreach (var obj in setActiveWhenInteracted)
                {
                    print("setting " + obj + " active");
                    obj.SetActive(true);
                }
                
                 if (increaseOrbCap) _player.GetComponent<FollowingOrbs>().IncreaseMaxOrbs();
                _player.GetComponent<Entity>().health = _player.GetComponent<Entity>().maxHealth;
                _ = LightQuestPoint(false);
                SaveSystem.save.maxOrbCount = _player.GetComponent<FollowingOrbs>().MaximumOrbCount;
                SaveSystem.save.orbCount = _player.GetComponent<FollowingOrbs>().OrbCount;
                SaveSystem.SaveCheckpoint(SceneManager.GetActiveScene().buildIndex, saveIndex);

                
            }
            else
            {
                //this, for some reason, was removed. that was bad.
                if(!hintCooldown) _ = ShowObjectives();

                interacted = false;
            }
        }
        else if (SaveSystem.lastTimeSaved + 10 < Time.time)
        {
            SaveSystem.SaveCheckpoint(SceneManager.GetActiveScene().buildIndex, saveIndex);
        }
    }

    public override void PlayerProximityEnter()
    {
        switch (General.Player.Player.Instance.PlayerController.LastInput)
        {
            case PlayerController.Controller.Gamepad:
                currentCanvas = interactCanvasGamepad;
                break;
            case PlayerController.Controller.Keyboard:
                currentCanvas = interactCanvasKeyboard;
                break;
        }
        
        
        currentCanvas.gameObject.SetActive(quest.CanLight(this));
    }

    public override void PlayerProximityStay()
    {
        if (quest.CanLight(this))
        {
            currentCanvas.LookAt(_camera);
            currentCanvas.localRotation *= Quaternion.Euler(0, 180, 0);
        }
        
    }

    private bool closeLastFrame;
    private void Update()
    {
        if (_player == null)
        {
            _player = Player.Instance.transform;
            if (_player == null) return;
        }
        
        bool close = Vector3.Distance(_player.position, transform.position) < 5 && !interacted;

        if (close && !interacted)
        {
            if (questCamera != null)
            {
                questCamera.gameObject.SetActive(true);
            }

            conditionCanvas.gameObject.SetActive(true);

            // conditionCanvas.LookAt(_camera);

            interacted = interacted && _questPointOn;

            if (!closeLastFrame)
            {
        
                foreach (var child in _canvasChildren)
                {
                    Destroy(child);
                }
        
                _canvasChildren.Clear();

                for (int i = 0; i < quest.conditions.Length; i++)
                {
                    CDPrefab newPrefab = Instantiate(conditionDisplay, conditionCanvas).GetComponent<CDPrefab>();
                    ConditionDisplay progress = quest.conditions[i].GetProgress(this);
            
                    newPrefab.image.sprite = progress.image;
                    newPrefab.text.text = progress.currentCount + " / " + progress.totalCount;
                    newPrefab.text.color = (progress.currentCount >= progress.totalCount) ? Color.green : Color.red;
            
                    _canvasChildren.Add(newPrefab.gameObject);
                }
            }
        }
        else
        {
            conditionCanvas.gameObject.SetActive(false);
            // go back to normal camera angle
            if (questCamera != null)
            {
                questCamera.gameObject.SetActive(false);

            }
        }

        closeLastFrame = close;
    }

    public override void PlayerProximityExit()
    {
        currentCanvas?.gameObject.SetActive(false);
    }


    protected async Awaitable LightQuestPoint(bool skipAnimation = false)
    {
        //this is where visuals are performed.

        List<Transform> children = new List<Transform>();
        List<Vector3> originalSize = new List<Vector3>();
        foreach (Transform tran in lightUpArea.transform)
        {
            children.Add(tran);
            originalSize.Add(tran.localScale);
        }

        lightUpArea.SetActive(true);
        AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);


        OnCheckpointLit?.Invoke();
        if (particles != null) particles.Play(true);

        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            float realTime = curve.Evaluate(t);

            for (int i = 0; i < children.Count; i++)
            {
                children[i].localScale = Vector3.Lerp(Vector3.zero, originalSize[i], realTime);
            }

            await Awaitable.NextFrameAsync();
        }
    }


    protected async Awaitable ShowObjectives()
    {
        hintCooldown = true;

        List<Transform> objectives = new List<Transform>();

        foreach (var c in campfires)
        {
            objectives.Add(c.transform);
        }
        foreach (var t in torches)
        {
            objectives.Add(t.transform);
        }
        foreach (var a in artifacts)
        {
            objectives.Add(a.transform);
        }

        List<Vector3> directions = new();
        List<Transform> gameobjects = new();
        for (int i = 0; i < objectives.Count; i++)
        {
            directions.Add((objectives[i].position - (transform.position + new Vector3(0, heightOffset, 0)))
                .normalized);
            GameObject newDirectionObject = Instantiate(campfireDirectionPrefabs,
                transform.position + new Vector3(0, heightOffset, 0), Quaternion.Euler(-90, 0, 0), transform);
            
            gameobjects.Add(newDirectionObject.transform);
        }
        
        for (float t = 0; t < 1; t += Time.unscaledDeltaTime / directionDuration)
        {
            float time = directionCurve.Evaluate(t);
            
            for (int i = 0; i < objectives.Count; i++)
            {
                gameobjects[i].position = transform.position + new Vector3(0, heightOffset, 0) +
                                          directions[i] * Mathf.Lerp(0, directionLength, time);
            }
            await Awaitable.NextFrameAsync();
        }

        await Awaitable.WaitForSecondsAsync(1);

        foreach (var o in gameobjects)
        {
            Destroy(o.gameObject);
        }
        hintCooldown = false;
    }
}