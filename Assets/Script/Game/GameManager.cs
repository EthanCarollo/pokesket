using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private BasketTeam[] teams;
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
#if UNITY_EDITOR
        if (IsLaunchedDirectly())
        {
            Debug.Log("==== Scène lancée directement depuis l'éditeur ! Lancement du match avec des paramètres prédéfinis.");
            StartMatch(
                new List<Pokemon>()
                {
                    PokemonDatabase.Instance.pokemons[10],
                    PokemonDatabase.Instance.pokemons[1],
                    PokemonDatabase.Instance.pokemons[0],
                },
                new List<Pokemon>()
                {
                    PokemonDatabase.Instance.pokemons[1],
                    PokemonDatabase.Instance.pokemons[4],
                    PokemonDatabase.Instance.pokemons[5],
                }
                );
        }
#endif
    }

#if UNITY_EDITOR
    private bool IsLaunchedDirectly()
    {
        return SceneManager.sceneCount == 1;
    }
#endif

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

    public BasketTeam GetTeam(TeamName teamName)
    {
        return teams[(int)teamName];
    }
}