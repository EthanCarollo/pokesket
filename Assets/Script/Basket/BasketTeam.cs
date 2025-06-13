using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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

    public List<PokemonPlayer> pokeTeam;
    public Image[] pokemonImages;
    private PokemonPlayer controlledPlayer;

    public void StartMatch()
    {
        SetControlledPlayer(pokeTeam[0]);
        for (int i = 0; i < pokeTeam.Count; i++)
        {
            pokemonImages[i].sprite = pokeTeam[i].actualPokemon.pokemonPortrait;
        }
    }

    void Update()
    {
        if (controlledPlayer != null)
        {
            if (Input.GetKeyDown(controlledPlayer.ControlledByPlayer1 ? XboxInput.RB1 : XboxInput.RB2))
            {
                if (!controlledPlayer?.HasBall ?? false)
                {
                    SwitchControlledPlayer();
                }
            }
        }
    }

    void SwitchControlledPlayer()
    {
        int currentIndex = pokeTeam.IndexOf(controlledPlayer);
        int nextIndex = (currentIndex + 1) % pokeTeam.Count;
        SetControlledPlayer(pokeTeam[nextIndex]);
    }

    public void SetControlledPlayer(PokemonPlayer newControlled)
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
