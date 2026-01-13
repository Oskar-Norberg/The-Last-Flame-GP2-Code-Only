using UnityEngine;
using System.Collections;

public class Blizzard : Hitbox
{
    public float damageTickSpeed = 2;
    public float size = 40;

    private ParticleSystem _snowParticles;
    private ParticleSystemForceField _particleForce;
    private CapsuleCollider _collider;

    public LayerMask cover;

    public float simulationSpeed = 1;
    public float windSpeed;
    public Vector2 windDirection;
    public float rotationOnWindSpeed = 6;


    private Transform playerPos;

    protected override void OnValidate()
    {
        base.OnValidate();

        if(_snowParticles == null) _snowParticles = GetComponentInChildren<ParticleSystem>();
        if(_particleForce == null) _particleForce = GetComponentInChildren<ParticleSystemForceField>();
        if(_collider == null) _collider = GetComponentInChildren<CapsuleCollider>();


        var main = _snowParticles.main;
        main.simulationSpeed = simulationSpeed;

        //Size
        _collider.radius = size;
        _particleForce.endRange = size*2f;
        var shape = _snowParticles.shape;
        shape.radius = size;

        //Wind
        windDirection = windDirection.normalized;
        _particleForce.directionX = windDirection.x * windSpeed;
        _particleForce.directionZ = windDirection.y * windSpeed;

        //Rotation based on wind speed and direction
        //Its to make sure that the particles are correctly placed when spawning
        transform.rotation = Quaternion.Euler(
            -windDirection.y * windSpeed / rotationOnWindSpeed, 0, 
            windDirection.x * windSpeed / rotationOnWindSpeed);
        _particleForce.transform.rotation = Quaternion.identity;

    }

    Coroutine damageRoutine = null;

    protected override void Enter(Entity e)
    {
        if(CanHit(e))
        {
            playerPos = e.transform;
            damageRoutine = StartCoroutine(DamagePlayer(e));
        }
    }
    protected override void Exit(Entity e)
    {
        if (CanHit(e))
        {
            StopCoroutine(damageRoutine);
            playerPos = null;
            damageRoutine = null;
        }
    }
    public IEnumerator DamagePlayer(Entity player)
    {
        while (true)
        {
            if (!CheckForShelter(player))
                Hit(player);

            yield return new WaitForSeconds(damageTickSpeed);
        }
    }

    protected override bool CanHit(Entity toHit)
    {
        return toHit is PlayerEntity;
    }

    public bool CheckForShelter(Entity e)
    {
        //make a raycast upwards and see if it collides with anything. if it does then dont deal damage
        return Physics.Raycast(e.transform.position + Vector3.up*0.5f, Vector3.up, 10f, cover) || 
            Physics.Raycast(e.transform.position + Vector3.up * 0.5f, new Vector3(windDirection.x, 0, windDirection.y), 5f, cover);
    }


    private void OnDrawGizmos()
    {
        if(playerPos != null)
        {
            Vector3 from = playerPos.position + Vector3.up * 0.5f;
            Gizmos.DrawLine(from, from + new Vector3(windDirection.x, 0, windDirection.y) * 5f);
            Gizmos.DrawLine(from, from + Vector3.up * 10f);
        }
    }
}
