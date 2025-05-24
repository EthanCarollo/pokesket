using UnityEngine;

public class AttackState : IPokemonPlayerState
{
    private PokemonPlayer _pokemonPlayer;
    
    public AttackState(PokemonPlayer pokemonPlayer)
    {
        _pokemonPlayer = pokemonPlayer;
    }

    public void Update()
    {
        if(!_pokemonPlayer.hasBall)
        {
            _pokemonPlayer.UpdateState(new DefenseState(_pokemonPlayer));
            return;
        }
        
        _pokemonPlayer.HandleMovement();
    }
}