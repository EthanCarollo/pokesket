using UnityEngine;

public class DefenseState : IPokemonPlayerState
{
    private PokemonPlayer _pokemonPlayer;
    
    public DefenseState(PokemonPlayer pokemonPlayer)
    {
        _pokemonPlayer = pokemonPlayer;
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