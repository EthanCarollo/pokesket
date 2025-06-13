using System.Collections;
using UnityEngine;

public class AIAttackState : IPokemonPlayerState
{
    private PokemonPlayer _pokemonPlayer;
    private float speed { get { return _pokemonPlayer.speed; }}

    public AIAttackState(PokemonPlayer pokemonPlayer)
    {
        _pokemonPlayer = pokemonPlayer;
        _pokemonPlayer.speed = speed;
        _pokemonPlayer.shootPlayer.enabled = false;
        _pokemonPlayer.passPlayer.enabled = false;
        _pokemonPlayer.blockShoot.enabled = false;
        _pokemonPlayer.blockPass.enabled = false;
    }

    private Vector3 nextPosition;
    private Vector3 movement;

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
        AIMovement();
    }

    public void AIMovement()
    {
        
    }

    public void HandleMovement()
    {
        Vector3 move = new Vector3(0, 0, 0);
        _pokemonPlayer.ApplyMovement(move);
    }
}