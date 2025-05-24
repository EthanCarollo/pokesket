using System;
using UnityEngine;

public class PokemonPlayer : MonoBehaviour
{
    bool isHumanControlled => GameManager.Instance.playedPokePlayerBlue == this;
    public bool hasBall => GameManager.Instance.currentBasketBallHolder == this;

    public BasketTeam team;
    public float speed = 5f;

    [NonSerialized]
    Vector3 lastMoveDirection = Vector3.up;
    public Vector3 Direction => lastMoveDirection;

    private IPokemonPlayerState currentState;

    private void Start()
    {
        currentState = new DefenseState(this);
    }

    void Update()
    {
        if (isHumanControlled)
        {
            currentState?.Update();
        }
    }

    public void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, v, 0);
        if (move.sqrMagnitude > 0.01f)
            lastMoveDirection = move.normalized;

        transform.Translate(move * speed * Time.deltaTime);
    }

    public void UpdateState(IPokemonPlayerState newState)
    {
        currentState = newState;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        BasketBall ball = other.GetComponent<BasketBall>();
        if (ball != null)
        {
            GameManager.Instance.currentBasketBallHolder = this;
        }
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

        Vector3 labelPosition = transform.position + Vector3.down * 0.2f;
        UnityEditor.Handles.Label(labelPosition, currentState.ToString(), style);
#endif
    }


}