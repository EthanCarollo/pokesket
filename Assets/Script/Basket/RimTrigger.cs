using UnityEngine;

public class RimTrigger : MonoBehaviour
{
    public enum TriggerType { First, Last }
    public TriggerType triggerType;
    public BasketRim parentRim;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            parentRim.OnBallEnter(triggerType);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            parentRim.OnBallExit(triggerType);
        }
    }
}