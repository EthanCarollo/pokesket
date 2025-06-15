using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum TeamName { Blue, Red }

public class BasketTeam : MonoBehaviour
{
    // Base info
    [SerializeField] public TeamName teamName;
    [SerializeField] public Transform rim;
    [SerializeField] private GameObject UISelectedCharacter;

    // Player pokemon part
    public List<PokemonPlayer> pokeTeam;
    public Image[] pokemonImages;
    public TeamName opponentTeamName => teamName == TeamName.Blue ? TeamName.Red : TeamName.Blue;
    [NonSerialized] public PokemonPlayer controlledPlayer;

    // Tactic part
    public GameObject TopZone;
    public GameObject FrontZone;
    public GameObject BottomZone;
    
    // Score part
    [SerializeField] private TextMeshProUGUI teamScoreText;
    private int _teamScore = 0;
    public int teamScore
    {
        get => _teamScore;
        set 
        {
            // Whenever we set team score, it update the TextMeshPro for the score
            teamScoreText.text = value.ToString();
            _teamScore = value;
        }
    }

    public void StartMatch()
    {
        SetControlledPlayer(pokeTeam[0]);
        for (int i = 0; i < pokeTeam.Count; i++)
        {
            pokemonImages[i].sprite = pokeTeam[i].actualPokemon.pokemonPortrait;
        }
        pokeTeam[0].role = PokemonRole.Front;
        pokeTeam[1].role = PokemonRole.Top;
        pokeTeam[2].role = PokemonRole.Bottom;
    }

    void Update()
    {
        if (controlledPlayer != null)
        {
            if (Input.GetKeyDown(controlledPlayer.ControlledByPlayer1 ? RemoteInput.RB1 : RemoteInput.RB2)) // RB on Xbox
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
        
        Image image = UISelectedCharacter.GetComponentInChildren<Image>();
        TextMeshProUGUI text = UISelectedCharacter.GetComponentInChildren<TextMeshProUGUI>();
        image.sprite = newControlled.actualPokemon.pokemonPortrait;
        text.text = newControlled.actualPokemon.pokemonName;
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