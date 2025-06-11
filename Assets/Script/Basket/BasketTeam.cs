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

    public List<PokemonPlayer> pokeTeam;
    private PokemonPlayer controlledPlayer;

    public void StartMatch()
    {
        SetControlledPlayer(pokeTeam[0]);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.JoystickButton7)) // RB on Xbox
        {
            if (!controlledPlayer?.HasBall ?? false)
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
