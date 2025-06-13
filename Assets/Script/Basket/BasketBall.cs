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

    public void ShootTowardsBasket(Vector3 target, float precision, float shootingQuality)
    {
        rb.useGravity = true;
        rb.isKinematic = false;

        Vector3 start = transform.position;

        if (shootingQuality > 0.8f)
        {
            StartEmitTrail();
            LeanTween.delayedCall(2.5f, () => StopEmitTrail());
        }

        float distance = Vector3.Distance(start, target);
        float effectivePrecision = precision * shootingQuality / 100f;
        bool isSuccessful = Random.value <= Mathf.Clamp01(effectivePrecision);

        Debug.LogWarning("Is shoot successful: " + isSuccessful);

        if (!isSuccessful)
        {
            // Tir raté complet → grosse déviation
            float missRadius = 1.0f * (1f - effectivePrecision) * (2f - shootingQuality);
            Vector2 offset2D = Random.insideUnitCircle.normalized * Random.Range(0.2f, missRadius);
            Vector3 offset = new Vector3(offset2D.x, 0, offset2D.y);
            target += offset;
        }
        else
        {
            // Tir "réussi", mais potentiellement imprécis selon distance et timing
            float distancePenaltyFactor = 0.1f;
            float distancePenalty = distance * (1f - effectivePrecision) * distancePenaltyFactor;

            if (distancePenalty > 0.01f)
            {
                Vector2 microOffset2D = Random.insideUnitCircle.normalized * Random.Range(0f, distancePenalty);
                Vector3 microOffset = new Vector3(microOffset2D.x, 0, microOffset2D.y);
                target += microOffset;
            }
        }

        // NE PAS multiplier la vitesse — cela casse la trajectoire !
        Vector3 velocity = CalculateArcVelocity(start, target);
        rb.linearVelocity = velocity;
    }

    private Vector3 CalculateArcVelocity(Vector3 start, Vector3 end)
    {
        float gravity = Mathf.Abs(Physics.gravity.y);

        Vector3 displacementXZ = new Vector3(end.x - start.x, 0, end.z - start.z);
        float distance = displacementXZ.magnitude;

        // Hauteur max = point le plus haut + arc en fonction de la distance
        float arcFactor = 0.3f;               // Influence de la distance sur la hauteur de la cloche
        float minArcExtra = 1.0f;             // Hauteur minimale ajoutée à l'apex
        float maxArcExtra = 2.5f;             // Hauteur maximale ajoutée à l'apex
        float arcExtra = Mathf.Clamp(distance * arcFactor, minArcExtra, maxArcExtra);

        float apexHeight = Mathf.Max(start.y, end.y) + arcExtra;

        // Temps vers le sommet
        float timeToApex = Mathf.Sqrt(2 * (apexHeight - start.y) / gravity);
        float timeFromApex = Mathf.Sqrt(2 * (apexHeight - end.y) / gravity);
        float totalTime = timeToApex + timeFromApex;

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

        // On force une passe tendue : apex très bas
        float arcFactor = 0.05f;             // Influence de la distance sur la hauteur de l’arc de passe
        float minArcHeight = 0.5f;          // Hauteur minimale de l’arc (pour les passes très courtes)
        float maxArcHeight = 1.2f;          // Hauteur maximale de l’arc (même pour les longues passes)
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