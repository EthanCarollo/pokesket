using System;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class BasketBallManager : MonoBehaviour
{
    public static BasketBallManager Instance;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform ballSpawnPoint;
    [SerializeField] private float timeBeforeReset = 3f;
    private BasketBall basketBall;
    private BasketTeam lastTeamHolder = null;
    private PokemonPlayer ballHolder = null;
    public PokemonPlayer BallHolder => ballHolder;
    private float lastTimeBlocked = -1f;
    
    [NonSerialized]
    public PokemonType lastPokemonTypeHolder;

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
            lastPokemonTypeHolder = holder.actualPokemon.pokemonType; 
            lastTeamHolder = holder.Team;
        }
    }

    public void ResetTeamHolder()
    {
        lastTeamHolder = null;
    }

    public bool IsBallHolded()
    {
        return ballHolder != null;
    }

    public bool IsTeamHoldingBall(BasketTeam team)
    {
        return lastTeamHolder == team;
    }

    public bool IsPlayerHoldingBall(PokemonPlayer player)
    {
        return ballHolder == player;
    }

    public void ShootTo(Transform target, float precision, float shootingQuality)
    {
        SetBallHolder(null);
        basketBall.ShootTowardsBasket(target.position, precision, shootingQuality);
    }

    public void PassTo(Transform target)
    {
        SetBallHolder(null);
        basketBall.PassTo(target.position);
    }
}
