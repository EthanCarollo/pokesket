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
        // Actually those functions are shitty cause we have nothing in it but I swear to god this shit is gonna be 
        // usefull
        
        if(_pokemonPlayer.hasBall)
        {
            _pokemonPlayer.UpdateState(new AttackState(_pokemonPlayer));
            return;
        }
        
        _pokemonPlayer.HandleMovement();
    }
}