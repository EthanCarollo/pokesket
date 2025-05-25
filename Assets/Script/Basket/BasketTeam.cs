using System;
using System.Collections.Generic;
using UnityEngine;

public enum TeamName { Blue, Red }
[Serializable]
public class TeamRim
{
    public TeamName teamName;
    public Transform ownerRim;
}

public class BasketTeam : MonoBehaviour
{
    [SerializeField] public TeamName teamName;
    [SerializeField] private Transform[] teamSpawnPoints;
    [SerializeField] private GameObject[] pokemonPrefabs;

    private List<PokemonPlayer> pokeTeam = new();
    private PokemonPlayer controlledPlayer;

    public void StartMatch()
    {
        for (int i = 0; i < teamSpawnPoints.Length; i++)
        {
            GameObject pokemon = Instantiate(pokemonPrefabs[i % pokemonPrefabs.Length], teamSpawnPoints[i].position, teamSpawnPoints[i].rotation);
            PokemonPlayer pokemonPlayer = pokemon.GetComponent<PokemonPlayer>();
            pokemonPlayer.Team = this;
            pokeTeam.Add(pokemonPlayer);
            if (i == 0)
            {
                SetControlledPlayer(pokemonPlayer);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.JoystickButton7)) // RB on Xbox
        {
            if (!controlledPlayer.HasBall)
            {
                SwitchControlledPlayer();
            }
        }
    }

    void SwitchControlledPlayer()
    {
        int currentIndex = pokeTeam.IndexOf(controlledPlayer);
        int nextIndex = (currentIndex + 1) % pokeTeam.Count;
        SetControlledPlayer(pokeTeam[nextIndex]);
    }

    void SetControlledPlayer(PokemonPlayer newControlled)
    {
        controlledPlayer = newControlled;
    }

    public bool IsControlled(PokemonPlayer player)
    {
        return player == controlledPlayer;
    }

    public Transform GetTargetRim()
    {
        TeamName opponentName = teamName == TeamName.Blue ? TeamName.Red : TeamName.Blue;
        return GameManager.Instance.GetTeamRim(opponentName).ownerRim;
    }
}
