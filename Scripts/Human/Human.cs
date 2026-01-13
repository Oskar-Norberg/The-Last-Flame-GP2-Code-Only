using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Freezable))]
public class Human : MonoBehaviour, IUpdateable
{
    private BehaviourTree _behaviourTree;

    public HumanSO HumanScriptableObject => humanScriptableObject;

    [SerializeField] private HumanSO humanScriptableObject;
    [SerializeField] private UnityEvent onRestSpotActivated;

    public delegate void OnHumanInstantiatedEventHandler(Human human);

    public static event OnHumanInstantiatedEventHandler OnHumanInstantiated;

    public delegate void OnHumanDestroyedEventHandler(Human human);

    public static event OnHumanDestroyedEventHandler OnHumanDestroyed;

    private void Awake()
    {
        if (humanScriptableObject == null)
        {
            Debug.LogError("Human scriptable object is not set!");
            enabled = false;
            return;
        }

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        Freezable freezable = GetComponent<Freezable>();

        _behaviourTree = new BehaviourTree(
            new Selector(new List<Node>
                {
                    new IsFrozen(freezable, new StopNavMeshAgent(agent)),

                    new IsRestSpotFound(new Sequence(new List<Node>
                    {
                        new SetNavMeshTargetRestSpot(agent),
                        new SetNavMeshSpeed(agent, humanScriptableObject.walkingSpeed,
                            humanScriptableObject.walkingAcceleration),
                        new StartNavMeshAgent(agent)
                    })),

                    new IsCloseToRestSpace(transform, humanScriptableObject.restSpotDetectionRange,
                        new Sequence(new List<Node>
                        {
                            new TargetRestSpot(transform),
                            new SetClosestRestSpaceAsRotationTarget()
                        })),

                    new IsPlayerDetected(new Selector(new List<Node>
                    {
                        new IsFarFromPlayer(transform, humanScriptableObject.playerForgetRange,
                            new MarkPlayerUndetected()),

                        new IsWithinRangeToPlayer(transform, humanScriptableObject.playerAvoidanceRange,
                            humanScriptableObject.playerAvoidanceMaxRange, new Sequence(new List<Node>
                            {
                                new SetPlayerAsRotationTarget(),
                                new StopNavMeshAgent(agent),
                            })),

                        new Sequence(new List<Node>
                        {
                            new Selector(new List<Node>
                            {
                                new IsCloseToPlayer(transform, humanScriptableObject.playerAvoidanceRange,
                                    new Sequence(new List<Node>
                                    {
                                        new TargetAwayFromPlayer(transform,
                                            (humanScriptableObject.playerAvoidanceRange +
                                             humanScriptableObject.playerAvoidanceMaxRange) / 2.0f),
                                        new SetNavMeshSpeed(agent, humanScriptableObject.playerAvoidanceSpeed,
                                            humanScriptableObject.playerAvoidanceAcceleration)
                                    })),

                                new Sequence(new List<Node>
                                {
                                    new TargetPlayer(),
                                    new SetNavMeshSpeed(agent, humanScriptableObject.walkingSpeed,
                                        humanScriptableObject.walkingAcceleration),
                                })
                            }),

                            new IsNavMeshDestinationFarFromTarget(agent,
                                humanScriptableObject.navmeshDistanceSetThreshold, new SetNavmeshTarget(agent)),
                            new StartNavMeshAgent(agent),
                        }),
                    })),

                    // If the player is not detected
                    new IsCloseToPlayer(transform, humanScriptableObject.playerDetectionRange,
                        new MarkPlayerDetected()),

                    new StopNavMeshAgent(agent)
                }
            ));

        agent.updateRotation = false;
        agent.angularSpeed = humanScriptableObject.rotationSpeed;
    }

    private void OnEnable()
    {
        OnHumanInstantiated?.Invoke(this);
    }

    private void OnDisable()
    {
        OnHumanDestroyed?.Invoke(this);
    }

    public void CustomUpdate(float deltaTime)
    {
        _behaviourTree.UpdateTree();
    }

    public void CustomFixedUpdate(float fixedDeltaTime)
    {
    }

    public void Rest()
    {
        onRestSpotActivated?.Invoke();
    }

    public BlackBoard GetBlackBoard()
    {
        return _behaviourTree.BlackBoard;
    }
}