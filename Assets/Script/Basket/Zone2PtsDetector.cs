using UnityEngine;

public class Zone2PtsDetector : MonoBehaviour
{
    public int collidersInZone { get; private set; } = 0;
    public TeamName? teamZone { get; private set; } = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("2pts"))
        {
            collidersInZone++;

            if (collidersInZone == 1)
            {
                teamZone = other.transform.parent.name.Contains("Red") ? TeamName.Red : TeamName.Blue;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("2pts"))
        {
            collidersInZone = Mathf.Max(0, collidersInZone - 1);

            if (collidersInZone == 0)
            {
                teamZone = null;
            }
        }
    }

    public bool IsInOpponent2PtsZone(TeamName selfTeam)
    {
        return teamZone != null && teamZone != selfTeam;
    }
}