using System.Collections;
using UnityEngine;

public class BallPossessionState : IPokemonPlayerState
{
    private PokemonPlayer _pokemonPlayer;
    private float speed = 5f;

    public BallPossessionState(PokemonPlayer pokemonPlayer)
    {
        _pokemonPlayer = pokemonPlayer;
        _pokemonPlayer.speed = speed;
        _pokemonPlayer.shootPlayer.enabled = true;
        _pokemonPlayer.passPlayer.enabled = true;
    }

    public void Update()
    {
        if (!_pokemonPlayer.HasBall)
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