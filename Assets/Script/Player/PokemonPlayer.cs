using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PokemonPlayer : MonoBehaviour
{
    public bool IsControlled => Team.IsControlled(this);
    public bool HasBall => BasketBallManager.Instance.IsPlayerHoldingBall(this);
    public bool TeamHasBall => BasketBallManager.Instance.IsTeamHoldingBall(Team);

    [Header("Stats")]
    public float speed = 5f;

    [NonSerialized]
    public Pokemon actualPokemon;
    public float shootPrecision
    {
        get => actualPokemon.shootPrecision;
    }

    [Header("References")]
    public BasketTeam Team;
    public bool ControlledByPlayer1
    {
        get { return Team.teamName == TeamName.Blue; }
    }
    [SerializeField] private SpriteRenderer indicator;
    [SerializeField] private SpriteRenderer pokemonSpriteRenderer;

    [NonSerialized] Vector3 lastMoveDirection = Vector3.up;
    public Vector3 Direction => lastMoveDirection;
    private bool _canHold = true;

    private IPokemonPlayerState currentState;
    
    public GameObject shootingUi;
    public Slider shootingSlider;
    public Image shootingSliderFill;
    public TextMeshProUGUI feedbackShootingText;

    [SerializeField]
    public PokemonPlayerAnimator pokemonPlayerAnimator;

    public void Start()
    {
        shootingUi.gameObject.SetActive(false);
    }

    public void Setup(Pokemon pokemon)
    {
        actualPokemon = pokemon;
        pokemonSpriteRenderer.sprite = pokemon.pokemonSprite;
        speed = pokemon.speed;
        currentState = new AIDefenseState(this);
        indicator.color = Team.teamName == TeamName.Red ? Color.red : Color.blue;
    }

    void Update()
    {
        if (GameManager.Instance.matchPlaying == false) return;

        currentState?.Update();
        currentState?.HandleMovement();
        
        indicator.gameObject.SetActive(IsControlled);
    }

    public void ApplyMovement(Vector3 move)
    {
        if (move.sqrMagnitude > 0.01f)
            lastMoveDirection = move.normalized;

        transform.Translate(move * speed * Time.deltaTime);
        
        try
        {
            pokemonPlayerAnimator.HandleAnimation(move, actualPokemon);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Error animating pokemon : " + actualPokemon.pokemonName + " : " + e);
        }
    }

    public void UpdateState(IPokemonPlayerState newState)
    {
        currentState = newState;
    }

    void OnTriggerEnter(Collider other)
    {
        bool isHolded = BasketBallManager.Instance.IsBallHolded();
        if (!HasBall && !isHolded && _canHold)
        {
            BasketBall ball = other.GetComponent<BasketBall>();
            if (ball != null)
            {
                BasketBallManager.Instance.SetBallHolder(this);
                Team.SetControlledPlayer(this);
            }
        }
    }

    public void LoseBall()
    {
        StartCoroutine(LoseBallCoroutine());
    }

    IEnumerator LoseBallCoroutine()
    {
        _canHold = false;
        yield return new WaitForSeconds(0.5f);
        _canHold = true;
    }
    
    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (currentState == null) return;

        var style = new GUIStyle
        {
            normal = { textColor = Color.white },
            alignment = TextAnchor.UpperCenter,
            fontSize = 12
        };

        Vector3 labelPosition = transform.position + Vector3.up * 0.2f;
        UnityEditor.Handles.Label(labelPosition, currentState.ToString(), style);
#endif
    }
}