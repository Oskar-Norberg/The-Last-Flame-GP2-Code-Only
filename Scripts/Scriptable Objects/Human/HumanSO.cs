using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "HumanSO", menuName = "Scriptable Objects/HumanSO")]
public class HumanSO : ScriptableObject
{
    public float walkingAcceleration;
    public float walkingSpeed;
    public float playerAvoidanceAcceleration;
    public float playerAvoidanceSpeed;
    
    // Degrees per second
    public float rotationSpeed;
    
    public float playerDetectionRange;
    public float playerForgetRange;
    public float playerAvoidanceRange;
    public float playerAvoidanceMaxRange;

    public float restSpotDetectionRange;

    public float navmeshDistanceSetThreshold;
}
