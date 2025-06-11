using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private BasketTeam[] teams;
    [SerializeField] private TeamRim[] teamRims;
    public bool matchPlaying = false;

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
        // StartMatch();
    }

    public void StartMatch(List<Pokemon> pokeTeamBlue, List<Pokemon> pokeTeamRed)
    {
        BasketBallManager.Instance.StartMatch();
        for (int i = 0; i < pokeTeamBlue.Count; i++)
        {
            var pokemon = pokeTeamBlue[i];
            teams[0].pokeTeam[i].Team = teams[0];
            teams[0].pokeTeam[i].Setup(pokemon);
        }

        for (int i = 0; i < pokeTeamRed.Count; i++)
        {
            var pokemon = pokeTeamRed[i];
            teams[1].pokeTeam[i].Team = teams[1];
            teams[1].pokeTeam[i].Setup(pokemon);
        }

        foreach (BasketTeam team in teams)
        {
            team.StartMatch();
        }
        matchPlaying = true;
    }

    public TeamRim GetTeamRim(TeamName teamName)
    {
        return teamRims[(int)teamName];
    }
}