using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class BasketBall : MonoBehaviour
{
    private PokemonPlayer currentHolder => BasketBallManager.Instance.BallHolder;

    public ParticleSystem particle;
    public TrailRenderer trailRenderer;
    [NonSerialized] public bool inZoneAtShoot = false;
    private bool inZone = false;

    public Rigidbody rb => GetComponent<Rigidbody>();

    public void Start()
    {
        StopEmitTrail();
    }

    public void StopEmitTrail()
    {
        particle.emissionRate = 0;
        trailRenderer.emitting = false;
    }

    public void StartEmitTrail()
    {
        particle.emissionRate = 12;
        trailRenderer.emitting = true;
    }

    public void Update()
    {
        if (currentHolder != null)
        {
            Vector3 offset = currentHolder.Direction * 0.5f;
            transform.position = currentHolder.transform.position + offset;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (currentHolder == null)
            {
                BasketBallManager.Instance.ResetTeamHolder();
                StopEmitTrail();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("2pts"))
        {
            inZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("2pts"))
        {
            inZone = false;
        }
    }

    public void ShootTowardsBasket(Vector3 target, bool isSuccessful, float force)
    {
        inZoneAtShoot = inZone;
        rb.useGravity = true;
        rb.isKinematic = false;

        Vector3 start = transform.position;

        if (force >= 0.95f)
        {
            StartEmitTrail();
            LeanTween.delayedCall(2.5f, () => StopEmitTrail());
        }

        Debug.LogWarning("Is shoot successful: " + isSuccessful);

        if (!isSuccessful)
        {
            float missRadius = 1.0f * (1f - Random.value);
            Vector2 offset2D = Random.insideUnitCircle.normalized * Random.Range(0.2f, missRadius);
            Vector3 offset = new Vector3(offset2D.x, 0, offset2D.y);
            target += offset;
        }

        Vector3 velocity = CalculateArcVelocity(start, target, force);
        rb.linearVelocity = velocity;
    }

    private Vector3 CalculateArcVelocity(Vector3 start, Vector3 end, float force)
    {
        float gravity = Mathf.Abs(Physics.gravity.y);

        Vector3 displacementXZ = new Vector3(end.x - start.x, 0, end.z - start.z);
        float distance = displacementXZ.magnitude;

        float arcFactor = 0.3f;
        float minArcExtra = 1.0f;
        float maxArcExtra = 2.5f;
        float arcExtra = Mathf.Clamp(distance * arcFactor, minArcExtra, maxArcExtra);

        float apexHeight = Mathf.Max(start.y, end.y) + arcExtra;

        float timeToApex = Mathf.Sqrt(2 * (apexHeight - start.y) / gravity);
        float timeFromApex = Mathf.Sqrt(2 * (apexHeight - end.y) / gravity);
        float totalTime = (timeToApex + timeFromApex) / Mathf.Clamp(force, 0.1f, 2f);

        Vector3 velocityXZ = displacementXZ / totalTime;
        float velocityY = Mathf.Sqrt(2 * gravity * (apexHeight - start.y));

        return velocityXZ + Vector3.up * velocityY;
    }

    public void PassTo(Vector3 target)
    {
        rb.useGravity = true;
        rb.isKinematic = false;

        Vector3 start = transform.position;
        Vector3 velocity = CalculatePassVelocity(start, target);

        rb.linearVelocity = velocity;
    }

    private Vector3 CalculatePassVelocity(Vector3 start, Vector3 end)
    {
        float gravity = Mathf.Abs(Physics.gravity.y);

        Vector3 displacementXZ = new Vector3(end.x - start.x, 0, end.z - start.z);
        float horizontalDistance = displacementXZ.magnitude;

        float arcFactor = 0.05f;
        float minArcHeight = 0.5f;
        float maxArcHeight = 1.2f;
        float arcExtra = Mathf.Clamp(horizontalDistance * arcFactor, minArcHeight, maxArcHeight);

        float apexHeight = Mathf.Max(start.y, end.y) + arcExtra;

        float timeToApex = Mathf.Sqrt(2 * (apexHeight - start.y) / gravity);
        float timeFromApex = Mathf.Sqrt(2 * (apexHeight - end.y) / gravity);
        float totalTime = timeToApex + timeFromApex;

        Vector3 velocityXZ = displacementXZ / totalTime;
        float velocityY = Mathf.Sqrt(2 * gravity * (apexHeight - start.y));

        return velocityXZ + Vector3.up * velocityY;
    }
}