using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BasketBall : MonoBehaviour
{
    private PokemonPlayer currentHolder => BasketBallManager.Instance.BallHolder;
    private Rigidbody rb => GetComponent<Rigidbody>();
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

    public void GoDirectlyIn(Vector3 target, float precision)
    {
        StartCoroutine(WaitThenMoveToRim(target, precision));
    }

    private void OnCollisionEnter(Collision collision)
    {
        // On considère que le tir est terminé uniquement si on touche le sol ou l’anneau
        if (collision.collider.CompareTag("Ground") || collision.collider.CompareTag("Ring"))
        {
            _isShooting = false;
        }
    }

    private IEnumerator WaitThenMoveToRim(Vector3 target, float precision)
    {
        yield return null; // attendre une frame pour relâcher le "suivi"
        ShootTowardsBasket(target, precision);
    }

    public void ShootTowardsBasket(Vector3 target, float precision)
    {
        rb.useGravity = true;
        rb.isKinematic = false;

        Vector3 start = transform.position;

        // --- PRÉCISION ---
        bool isSuccessful = Random.value <= Mathf.Clamp01(precision);
        if (!isSuccessful)
        {
            // Ajoute une déviation de cible (max 1m) si le tir est raté
            float missRadius = 1.0f * (1f - precision); // plus on est imprécis, plus la déviation est grande

            Vector2 offset2D = Random.insideUnitCircle.normalized * Random.Range(0.2f, missRadius);
            Vector3 offset = new Vector3(offset2D.x, 0, offset2D.y); // décalage uniquement en XZ

            target += offset;
        }

        Vector3 velocity = CalculateArcVelocity(start, target);

        rb.linearVelocity = velocity;
        _isShooting = true;
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