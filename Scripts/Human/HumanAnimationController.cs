using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Human))]
public class HumanAnimationController : MonoBehaviour
{
    // TODO: In a larger scale this would be so ass, but for now its fine :3 - Oskar
    private static readonly int[] IdleTriggers =
    {
        Animator.StringToHash("IdleTrigger1"),
        Animator.StringToHash("IdleTrigger2"),
        Animator.StringToHash("IdleTrigger3"),
        Animator.StringToHash("IdleTrigger4"),
    };

    private static readonly int MovementBlend = Animator.StringToHash("MoveBlend");
    private static readonly int IsMoving = Animator.StringToHash("isMoving");

    [SerializeField] private float secondsBetweenIdleTriggers = 5.0f;
    [SerializeField] private float movementThreshold = 0.05f;

    [SerializeField] private Human human;
    [SerializeField] private Freezable freezable;
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent navMeshAgent;

    private float _secondsSinceLastIdleTrigger = 0.0f;
    private HumanSO _humanScriptableObject;

    private void Awake()
    {
        _humanScriptableObject = human.HumanScriptableObject;
    }

    private void Update()
    {
        if (SetFrozen())
            return;

        RotateTowardsTarget();

        // If player is moving, don't trigger idle animations
        if (SetVelocity())
            return;

        _secondsSinceLastIdleTrigger += Time.deltaTime;

        if (secondsBetweenIdleTriggers < _secondsSinceLastIdleTrigger)
            TriggerRandomIdle();
    }

    /**
     * <summary>
     * Sets animator frozen state based on freezable component.
     * </summary>
     * <returns>
     * Returns true if player is frozen, false otherwise.
     * </returns>
     */
    private bool SetFrozen()
    {
        if (!freezable.IsFrozen) return false;

        animator.SetFloat(MovementBlend, 0.0f);
        animator.SetBool(IsMoving, false);
        return true;
    }

    /**
     * <summary>
     * Sets animator velocity based on player movement.
     * </summary>
     * <returns>
     * Returns true if player is moving, false otherwise.
     * </returns>
     */
    private bool SetVelocity()
    {
        float currentVelocity = Mathf.Abs(navMeshAgent.velocity.magnitude);

        float maxVelocity = Mathf.Abs(_humanScriptableObject.walkingSpeed);

        float velocityThreshold = currentVelocity / maxVelocity;

        animator.SetFloat(MovementBlend, velocityThreshold);

        bool isMoving = currentVelocity > movementThreshold;

        animator.SetBool(IsMoving, currentVelocity > movementThreshold);

        return isMoving;
    }

    private void TriggerRandomIdle()
    {
        int randomIndex = Random.Range(0, IdleTriggers.Length);
        animator.SetTrigger(IdleTriggers[randomIndex]);
        _secondsSinceLastIdleTrigger = 0.0f;
    }

    private void RotateTowardsTarget()
    {
        if (!human.GetBlackBoard().TryGetData("TargetRotation", out object rotationTargetValue))
            return;

        Vector3 direction = GetTargetPosition(rotationTargetValue);

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation,
            Time.deltaTime * _humanScriptableObject.rotationSpeed);
    }

    /**
     * <summary>
     * Parses object from either a transform or Vector3 to a Vector3.
     * </summary>
     * <returns>
     * Returns a Vector3 in world space.
     * Returns Vector3.negativeInfinity if object is not a transform or Vector3.
     * </returns>
     */
    private Vector3 GetTargetPosition(object rotationTargetValue)
    {
        if (rotationTargetValue is Transform rotationTargetTransform)
            return (rotationTargetTransform.position - transform.position).normalized;
        if (rotationTargetValue is Vector3 rotationTargetVector && rotationTargetVector != Vector3.zero)
            return (rotationTargetVector - transform.position).normalized;

        return Vector3.negativeInfinity;
    }
}