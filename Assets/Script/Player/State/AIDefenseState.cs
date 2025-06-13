using UnityEngine;

public class AIDefenseState : IPokemonPlayerState
{
    private PokemonPlayer _pokemonPlayer;
    private float speed = 8f;

    public AIDefenseState(PokemonPlayer pokemonPlayer)
    {
        _pokemonPlayer = pokemonPlayer;
        _pokemonPlayer.speed = speed;
        _pokemonPlayer.shootPlayer.enabled = false;
        _pokemonPlayer.passPlayer.enabled = false;
        _pokemonPlayer.blockShoot.enabled = false;
        _pokemonPlayer.blockPass.enabled = false;
    }

    public void Update()
    {
        if (_pokemonPlayer.IsControlled)
        {
            _pokemonPlayer.UpdateState(new PlayerDefenseState(_pokemonPlayer));
            return;
        }

        if (_pokemonPlayer.TeamHasBall)
        {
            _pokemonPlayer.UpdateState(new AIAttackState(_pokemonPlayer));
            return;
        }
    }
    
    public void HandleMovement()
    {
        
    }
}