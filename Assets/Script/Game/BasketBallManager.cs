using System;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class BasketBallManager : MonoBehaviour
{
    public static BasketBallManager Instance;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform ballSpawnPoint;
    [SerializeField] private float timeBeforeReset = 3f;
    public BasketBall basketBall;
    [NonSerialized] public PokemonPlayer lastBallHolder = null;
    private PokemonPlayer ballHolder = null;
    public PokemonPlayer BallHolder => ballHolder;
    private float lastTimeBlocked = -1f;

    public BasketTeam lastTeamHolder => lastBallHolder?.Team;
    public PokemonType lastPokemonTypeHolder => lastBallHolder?.actualPokemon.pokemonType;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        if (GameManager.Instance.matchPlaying == false) return;
        if (basketBall.rb.linearVelocity == Vector3.zero && basketBall.transform.position.y > 0.1f)
        {
            if (lastTimeBlocked == -1f)
            {
                lastTimeBlocked = Time.time;
            }
            else if (Time.time - lastTimeBlocked >= timeBeforeReset)
            {
                lastTimeBlocked = -1f;
                ResetBasketBall();
            }
        }
    }

    public void ResetBasketBall()
    {
        Destroy(basketBall.gameObject);
        StartMatch();
    }

    public void StartMatch()
    {
        basketBall = Instantiate(ballPrefab, ballSpawnPoint.position, ballSpawnPoint.rotation).GetComponent<BasketBall>();
    }

    public void SetBallHolder(PokemonPlayer holder)
    {
        ballHolder = holder;
        if (holder != null)
        {
            lastBallHolder = holder;
            if (holder.Team.teamName == TeamName.Blue)
            {
                GameManager.Instance.CameraManager.SetNewLookAtTransform(
                    GameManager.Instance.CameraManager.blueCameraTarget.transform,
                    new Vector3(0f, 9f, -19f)
                    );
            }
            else
            {
                GameManager.Instance.CameraManager.SetNewLookAtTransform(
                    GameManager.Instance.CameraManager.redCameraTarget.transform,
                    new Vector3(0f, 9f, -19f)
                    );
            }
        }
        else
        {
            // To avoid "The gerbe" for the User
            // GameManager.Instance.CameraManager.ResetPositionAndLookAt();
        }
    }

    public void ResetHolder()
    {
        lastBallHolder = null;
    }

    public bool IsBallHolding()
    {
        return ballHolder != null;
    }

    public bool IsBallHolded()
    {
        return lastBallHolder != null;
    }

    public bool IsTeamHoldedBall(BasketTeam team)
    {
        return lastTeamHolder == team;
    }

    public bool IsPlayerHoldingBall(PokemonPlayer player)
    {
        return ballHolder == player;
    }

    public void ShootTo(Transform target, bool isSuccessful, float force, Pokemon shooter)
    {
        SetBallHolder(null);
        basketBall.ShootTowardsBasket(target.position, isSuccessful, force, shooter);
    }

    public void PassTo(Transform target)
    {
        SetBallHolder(null);
        basketBall.PassTo(target.position);
    }
    
    public void DunkTo(Transform target)
    {
        SetBallHolder(null);
        basketBall.DunkInto(target.position);
    }
}
