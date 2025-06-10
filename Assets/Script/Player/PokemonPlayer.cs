using System;
using System.Collections;
using UnityEngine;

public class PokemonPlayer : MonoBehaviour
{
    public bool IsControlled => Team.IsControlled(this);
    public bool HasBall => BasketBallManager.Instance.IsPlayerHoldingBall(this);

    [Header("Stats")]
    public float speed = 5f;
    [Range(0f, 1f)] public float precision = 0.5f;
    [Header("References")]
    public BasketTeam Team;
    [SerializeField] private SpriteRenderer indicator;

    [NonSerialized] Vector3 lastMoveDirection = Vector3.up;
    public Vector3 Direction => lastMoveDirection;
    private bool _isPassing = false;

    private IPokemonPlayerState currentState;

    public void Setup(Pokemon pokemon)
    {
        speed = pokemon.speed;
    }

    private void Start()
    {
        currentState = new DefenseState(this);
        indicator.color = Team.teamName == TeamName.Red ? Color.red : Color.blue;
    }

    void Update()
    {
        currentState?.Update();
        
        indicator.gameObject.SetActive(IsControlled);
    }

    public void HandleMovement()
    {
        if (IsControlled)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            Vector3 move = new Vector3(h, 0, v);
            if (move.sqrMagnitude > 0.01f)
                lastMoveDirection = move.normalized;

            transform.Translate(move * speed * Time.deltaTime);
        }
        else
        {
            // AI Movement
        }
    }


    public void UpdateState(IPokemonPlayerState newState)
    {
        currentState = newState;
    }

    void OnTriggerEnter(Collider other)
    {
        bool isHolded = BasketBallManager.Instance.IsBallHolded();
        if (!HasBall && !isHolded && !_isPassing)
        {
            BasketBall ball = other.GetComponent<BasketBall>();
            if (ball != null && !ball.IsShooting)
            {
                BasketBallManager.Instance.SetBallHolder(this);
                Team.SetControlledPlayer(this);
            }
        }
    }

    public IEnumerator Pass()
    {
        _isPassing = true;
        yield return new WaitForSeconds(0.5f);
        _isPassing = false;
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