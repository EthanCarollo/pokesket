using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum TeamName { Blue, Red }

public class BasketTeam : MonoBehaviour
{
    [SerializeField] public TeamName teamName;
    [SerializeField] public Transform rim;

    public List<PokemonPlayer> pokeTeam;
    public Image[] pokemonImages;
    public TeamName opponentTeamName => teamName == TeamName.Blue ? TeamName.Red : TeamName.Blue;
    [NonSerialized] public PokemonPlayer controlledPlayer;

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
            if (Input.GetKeyDown(controlledPlayer.ControlledByPlayer1 ? KeyCode.Joystick1Button7 : KeyCode.Joystick2Button7)) // RB on Xbox
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
        // Cherche le Pokémon (autre que le contrôlé) le plus proche de la balle
        PokemonPlayer nearestPokemon = pokeTeam
            .Where(p => p != controlledPlayer)
            .OrderBy(p => Vector3.Distance(p.transform.position, BasketBallManager.Instance.basketBall.transform.position))
            .FirstOrDefault();

        // Si aucun trouvé (rare), on passe au suivant dans la liste
        int nextIndex = (nearestPokemon != null)
            ? pokeTeam.IndexOf(nearestPokemon)
            : (pokeTeam.IndexOf(controlledPlayer) + 1) % pokeTeam.Count;

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

    public BasketTeam GetOpponentTeam()
    {
        return GameManager.Instance.GetTeam(opponentTeamName);
    }

    public Transform GetOpponentRim()
    {
        return GetOpponentTeam().rim;
    }
}