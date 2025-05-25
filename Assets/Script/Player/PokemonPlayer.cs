using System;
using UnityEngine;

public class PokemonPlayer : MonoBehaviour
{
    public bool IsControlled => Team.IsControlled(this);
    public bool HasBall => BasketBallManager.Instance.IsPlayerHoldingBall(this);

    public BasketTeam Team;
    public float speed = 5f;

    [NonSerialized] Vector3 lastMoveDirection = Vector3.up;
    public Vector3 Direction => lastMoveDirection;

    private IPokemonPlayerState currentState;

    private void Start()
    {
        currentState = new DefenseState(this);
    }

    void Update()
    {
        currentState?.Update();
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
        BasketBall ball = other.GetComponent<BasketBall>();
        if (ball != null)
        {
            BasketBallManager.Instance.SetBallHolder(this);
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

        Vector3 labelPosition = transform.position + Vector3.up * 0.2f;
        UnityEditor.Handles.Label(labelPosition, currentState.ToString(), style);
#endif
    }
}