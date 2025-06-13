using System.Collections;
using UnityEngine;

public class AIAttackState : IPokemonPlayerState
{
    private PokemonPlayer _pokemonPlayer;
    private float speed = 5f;

    public AIAttackState(PokemonPlayer pokemonPlayer)
    {
        _pokemonPlayer = pokemonPlayer;
        _pokemonPlayer.speed = speed;
        _pokemonPlayer.shootPlayer.enabled = false;
        _pokemonPlayer.passPlayer.enabled = false;
    }

    public void Update()
    {
        if (_pokemonPlayer.IsControlled)
        {
            _pokemonPlayer.UpdateState(new PlayerAttackState(_pokemonPlayer));
            return;
        }

        if (!_pokemonPlayer.TeamHasBall)
        {
            _pokemonPlayer.UpdateState(new AIDefenseState(_pokemonPlayer));
            return;
        }
    }

    public void HandleMovement()
    {
        
    }
}