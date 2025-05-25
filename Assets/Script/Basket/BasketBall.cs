using System.Collections;
using UnityEngine;

public class BasketBall : MonoBehaviour
{
    private PokemonPlayer currentHolder => BasketBallManager.Instance.BallHolder;

    public void Update()
    {
        if (currentHolder != null)
        {
            Vector3 offset = currentHolder.Direction * 0.5f;
            transform.position = currentHolder.transform.position + offset;
        }
    }

    public void GoDirectlyIn(Vector3 target)
    {
        StartCoroutine(WaitThenMoveToRim(target));
    }

    private IEnumerator MoveToRim(Vector3 target)
    {
        float duration = 0.6f;
        float time = 0f;

        Vector3 start = transform.position;

        // On ajuste la cible pour viser légèrement au-dessus et devant l’arceau
        Vector3 adjustedTarget = target + Vector3.up * 0.3f - transform.forward * 0.1f;

        // Positions projetées au sol (XZ uniquement)
        Vector3 horizontalStart = new Vector3(start.x, 0f, start.z);
        Vector3 horizontalTarget = new Vector3(adjustedTarget.x, 0f, adjustedTarget.z);

        float arcHeight = 2.5f;

        while (time < duration)
        {
            float t = time / duration;

            // Interpolation linéaire en XZ
            Vector3 horizontalPos = Vector3.Lerp(horizontalStart, horizontalTarget, t);

            // Calcul de la hauteur via une parabole (cloche)
            float height = Mathf.Sin(Mathf.PI * t) * arcHeight;

            // Position finale combinée
            transform.position = new Vector3(horizontalPos.x, adjustedTarget.y + height, horizontalPos.z);

            time += Time.deltaTime;
            yield return null;
        }

        transform.position = adjustedTarget;
    }
    
    private IEnumerator WaitThenMoveToRim(Vector3 target)
    {
        yield return null; // attendre une frame pour relâcher le "suivi"
        yield return MoveToRim(target);
    }
}