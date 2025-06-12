using TMPro;
using UnityEngine;

public class BasketRim : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI opponentScore;

    private bool hasEnteredFromTop = false;
    private bool hasScored = false;
    // When we hit the balls
    public GameObject particleParent;

    public void OnBallEnter(RimTrigger.TriggerType trigger)
    {
        if (trigger == RimTrigger.TriggerType.First)
        {
            // Entrée par le haut
            hasEnteredFromTop = true;
            hasScored = false;
        }
        else if (trigger == RimTrigger.TriggerType.Last)
        {
            // Si la balle est bien passée par le haut d’abord
            if (hasEnteredFromTop && !hasScored)
            {
                // if(particleParent.)
                Debug.LogWarning("Ball entered top");
                Debug.LogWarning(BasketBallManager.Instance.lastPokemonTypeHolder);
                Instantiate(BasketBallManager.Instance.lastPokemonTypeHolder.particlePointPrefab, particleParent.transform);
                Goal();
                hasScored = true; // Évite les scores multiples
            }
        }
    }

    public void OnBallExit(RimTrigger.TriggerType trigger)
    {
        if (trigger == RimTrigger.TriggerType.First)
        {
            // Si la balle ressort par le haut sans marquer, on annule
            hasEnteredFromTop = false;
            hasScored = false;
        }
    }

    private void Goal()
    {
        int current = int.Parse(opponentScore.text);
        opponentScore.text = (current + 2).ToString();
        Debug.Log("PANIER !");
    }
}