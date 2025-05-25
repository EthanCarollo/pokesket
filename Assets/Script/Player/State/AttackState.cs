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
        if (!_pokemonPlayer.HasBall)
        {
            _pokemonPlayer.UpdateState(new DefenseState(_pokemonPlayer));
            return;
        }

        _pokemonPlayer.HandleMovement();

        if (_pokemonPlayer.IsControlled)
        {
            if (Input.GetKeyDown(KeyCode.JoystickButton1)) // B on Xbox
            {
                LaunchBall();
            }
        }
        else
        {
            // AI Attack
        }
    }
    
    public void LaunchBall()
    {
        var rim = _pokemonPlayer.Team.GetTargetRim();
        BasketBallManager.Instance.ShootTo(rim);
    }
}