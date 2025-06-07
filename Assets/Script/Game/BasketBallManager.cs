using System;
using UnityEngine;

public class BasketBallManager : MonoBehaviour
{
    public static BasketBallManager Instance;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform ballSpawnPoint;
    [SerializeField] private float timeBeforeReset = 3f;
    private BasketBall basketBall;
    private PokemonPlayer ballHolder;
    public PokemonPlayer BallHolder => ballHolder;
    private float lastTimeBlocked = -1f;

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
    }

    public bool IsPlayerHoldingBall(PokemonPlayer player)
    {
        return ballHolder == player;
    }

    public void ShootTo(Transform target, float precision)
    {
        SetBallHolder(null);
        basketBall.ShootTowardsBasket(target.position, precision);
    }
}
