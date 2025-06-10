using System.Collections;
using UnityEngine;

public class AttackState : IPokemonPlayerState
{
    private PokemonPlayer _pokemonPlayer;
    private float speed = 5f;
    private bool _isPassing = false;

    public AttackState(PokemonPlayer pokemonPlayer)
    {
        _pokemonPlayer = pokemonPlayer;
        _pokemonPlayer.speed = speed;
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
            if (Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.B)) // B on Xbox
            {
                LaunchBall();
            }

            if (Input.GetKeyDown(KeyCode.JoystickButton3) || Input.GetKeyDown(KeyCode.X)) // X on Xbox
            {
                PassBall();
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
        BasketBallManager.Instance.ShootTo(rim, _pokemonPlayer.precision);
    }

    public void PassBall()
    {
        Vector2 inputDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 myPosition = _pokemonPlayer.transform.position;

        Vector3? direction = null;
        if (inputDir.sqrMagnitude >= 0.1f)
        {
            direction = new Vector3(inputDir.x, 0, inputDir.y).normalized;
        }

        PokemonPlayer bestTargetInDirection = null;
        float bestScoreInDirection = float.MaxValue;

        PokemonPlayer closestAlly = null;
        float closestDistance = float.MaxValue;

        float maxAngle = 60f;

        int teammateCount = 0;

        foreach (var ally in _pokemonPlayer.Team.PokeTeam)
        {
            if (ally == _pokemonPlayer) continue;
            teammateCount++;

            Vector3 toAlly = ally.transform.position - myPosition;
            float distance = toAlly.sqrMagnitude;

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestAlly = ally;
            }

            if (direction.HasValue)
            {
                float angle = Vector3.Angle(direction.Value, toAlly);
                if (angle > maxAngle) continue;

                if (distance < bestScoreInDirection)
                {
                    bestScoreInDirection = distance;
                    bestTargetInDirection = ally;
                }
            }
        }

        if (teammateCount == 0)
        {
            Debug.LogWarning("Aucun coéquipier autre que soi");
            return;
        }

        PokemonPlayer finalTarget = bestTargetInDirection ?? closestAlly;

        BasketBallManager.Instance.PassTo(finalTarget.transform);

        _pokemonPlayer.StartCoroutine(_pokemonPlayer.Pass());
    }
}