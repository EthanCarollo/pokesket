using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private BasketTeam[] teams;
    [SerializeField] private TeamRim[] teamRims;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        BasketBallManager.Instance.StartMatch();
        foreach (BasketTeam team in teams)
        {
            team.StartMatch();
        }
    }

    public TeamRim GetTeamRim(TeamName teamName)
    {
        return teamRims[(int)teamName];
    }
}