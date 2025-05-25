using UnityEngine;

public class DefenseState : IPokemonPlayerState
{
    private PokemonPlayer _pokemonPlayer;
    private float speed = 8f;

    public DefenseState(PokemonPlayer pokemonPlayer)
    {
        _pokemonPlayer = pokemonPlayer;
        _pokemonPlayer.speed = speed;
    }

    public void Update()
    {   
        if(_pokemonPlayer.HasBall)
        {
            _pokemonPlayer.UpdateState(new AttackState(_pokemonPlayer));
            return;
        }
        
        _pokemonPlayer.HandleMovement();
    }
}