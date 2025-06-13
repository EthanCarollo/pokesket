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
    public Light ballLight;
    [NonSerialized] public int points = 3;
    private int _collidersInZone = 0;
    private TeamName? teamZone = null;

    public Rigidbody rb => GetComponent<Rigidbody>();

    public void Start()
    {
        StopEmitTrail();
    }

    public void StopEmitTrail()
    {
        particle.emissionRate = 0;
        trailRenderer.emitting = false;
        ballLight.intensity = 0;
    }

    public void StartEmitTrail(PokemonType pokemonType)
    {
        particle.emissionRate = 12;
        particle.gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", pokemonType.noHdrTypeColor);
        trailRenderer.material.SetTexture("_TintTex", pokemonType.trailTexture);
        // .SetTexture("_TintTex", pokemonType.trailTexture);
        trailRenderer.emitting = true;
        ballLight.color = pokemonType.noHdrTypeColor;
        ballLight.intensity = 7.7f;
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
                BasketBallManager.Instance.ResetHolder();
                StopEmitTrail();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("2pts"))
        {
            _collidersInZone++;

            if (_collidersInZone == 1)
            {
                teamZone = other.gameObject.transform.parent.name.Contains("Red") ? TeamName.Red : TeamName.Blue;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("2pts"))
        {
            _collidersInZone = Mathf.Max(0, _collidersInZone - 1);

            if (_collidersInZone == 0)
            {
                teamZone = null;
            }
        }
    }

    public void ShootTowardsBasket(Vector3 target, bool isSuccessful, float force, Pokemon shooter)
    {
        points = teamZone == BasketBallManager.Instance.lastTeamHolder.teamName || teamZone == null ? 3 : 2;
        rb.useGravity = true;
        rb.isKinematic = false;

        Vector3 start = transform.position;

        if (force >= 0.95f)
        {
            StartEmitTrail(shooter.pokemonType);
            GameManager.Instance.CameraManager.SetNewLookAtTransform(this.transform);
            LeanTween.delayedCall(2.5f, () =>
            {
                StopEmitTrail();
            });
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