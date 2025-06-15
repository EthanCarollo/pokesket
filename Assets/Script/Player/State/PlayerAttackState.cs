using UnityEngine;

public class PlayerAttackState : IPokemonPlayerState
{
    private PokemonPlayer _pokemonPlayer;
    private float speed = 5f;

    public PlayerAttackState(PokemonPlayer pokemonPlayer)
    {
        _pokemonPlayer = pokemonPlayer;
        _pokemonPlayer.speed = speed;
        _pokemonPlayer.shootPlayer.enabled = false;
        _pokemonPlayer.passPlayer.enabled = false;
        _pokemonPlayer.dunkPlayer.enabled = false;
        _pokemonPlayer.blockShoot.enabled = false;
        _pokemonPlayer.blockPass.enabled = false;
    }

    public void Update()
    {
        if (!_pokemonPlayer.IsControlled)
        {
            _pokemonPlayer.UpdateState(new AIAttackState(_pokemonPlayer));
            return;
        }

        if (!_pokemonPlayer.TeamHasBall)
        {
            _pokemonPlayer.UpdateState(new PlayerDefenseState(_pokemonPlayer));
            return;
        }

        if (_pokemonPlayer.HasBall)
        {
            _pokemonPlayer.UpdateState(new BallPossessionState(_pokemonPlayer));
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