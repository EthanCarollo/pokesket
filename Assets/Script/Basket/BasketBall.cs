using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BasketBall : MonoBehaviour
{
    private PokemonPlayer currentHolder => BasketBallManager.Instance.BallHolder;
    public Rigidbody rb => GetComponent<Rigidbody>();
    [SerializeField]private bool _isShooting = false;
    public bool IsShooting => _isShooting;

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
        // On considère que le tir est terminé uniquement si on touche le sol ou l’anneau
        if (collision.collider.CompareTag("Ground") || collision.collider.CompareTag("Ring"))
        {
            _isShooting = false;
        }
    }

    public void ShootTowardsBasket(Vector3 target, float precision)
    {
        _isShooting = true;

        rb.useGravity = true;
        rb.isKinematic = false;

        Vector3 start = transform.position;
        Vector3 originalTarget = target;

        // Distance entre le tireur et le panier
        float distance = Vector3.Distance(start, target);

        // --- PRÉCISION PRINCIPALE ---
        bool isSuccessful = Random.value <= Mathf.Clamp01(precision);

        if (!isSuccessful)
        {
            // Tir raté complet → grosse déviation
            float missRadius = 1.0f * (1f - precision);
            Vector2 offset2D = Random.insideUnitCircle.normalized * Random.Range(0.2f, missRadius);
            Vector3 offset = new Vector3(offset2D.x, 0, offset2D.y);
            target += offset;
        }
        else
        {
            // Tir "réussi", mais potentiellement imprécis selon distance
            float distancePenaltyFactor = 0.1f; // combien la distance impacte l'imprécision (ajustable)
            float distancePenalty = distance * (1f - precision) * distancePenaltyFactor;

            if (distancePenalty > 0.01f)
            {
                // Ajoute une déviation mineure malgré réussite
                Vector2 microOffset2D = Random.insideUnitCircle.normalized * Random.Range(0f, distancePenalty);
                Vector3 microOffset = new Vector3(microOffset2D.x, 0, microOffset2D.y);
                target += microOffset;
            }
        }

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
}