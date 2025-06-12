using UnityEngine;

public class PlayerDefenseState : IPokemonPlayerState
{
    private PokemonPlayer _pokemonPlayer;
    private float speed = 8f;

    public PlayerDefenseState(PokemonPlayer pokemonPlayer)
    {
        _pokemonPlayer = pokemonPlayer;
        _pokemonPlayer.speed = speed;
    }

    public void Update()
    {
        if (!_pokemonPlayer.IsControlled)
        {
            _pokemonPlayer.UpdateState(new AIDefenseState(_pokemonPlayer));
            return;
        }

        if (_pokemonPlayer.TeamHasBall)
        {
            _pokemonPlayer.UpdateState(new PlayerAttackState(_pokemonPlayer));
            return;
        }
    }
    
    public void HandleMovement()
    {
        float h = _pokemonPlayer.ControlledByPlayer1 ? Input.GetAxis("HorizontalJoystick1") : Input.GetAxis("HorizontalJoystick2");
        float v = _pokemonPlayer.ControlledByPlayer1 ? Input.GetAxis("VerticalJoystick1") : Input.GetAxis("VerticalJoystick2");

        Vector3 move = new Vector3(h, 0, v);
        _pokemonPlayer.ApplyMovement(move);
    }
}