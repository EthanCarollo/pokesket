using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PokemonPlayer))]
public class BlockPass : MonoBehaviour
{
    [SerializeField] private float timingToReblock = 2f;
    [SerializeField] private float maxDashDistance = 2.5f;
    [SerializeField] private float dashDuration = 0.2f;

    private PokemonPlayer _pokemonPlayer;

    void Awake()
    {
        _pokemonPlayer = GetComponent<PokemonPlayer>();
    }

    void Update()
    {
        if (Input.GetKeyDown(_pokemonPlayer.ControlledByPlayer1 ? RemoteInput.A1 : RemoteInput.A2))
        {
            if (_pokemonPlayer.canBlock && BasketBallManager.Instance.IsBallHolded())
            {
                TryBlock();
            }
        }
    }

    private void TryBlock()
    {
        PokemonPlayer target = BasketBallManager.Instance.BallHolder;

        if (target != null && target != _pokemonPlayer)
        {
            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
            Vector3 dashTarget = transform.position + directionToTarget * maxDashDistance;
            StartCoroutine(DashTowards(dashTarget, success: true));
        }
        else
        {
            // Dash dans la direction actuelle du joueur (pénalité si raté)
            Vector3 fallbackDir = _pokemonPlayer.Direction.normalized;
            Vector3 dashTarget = transform.position + fallbackDir * maxDashDistance;
            StartCoroutine(DashTowards(dashTarget, success: false));
        }
    }

    private IEnumerator DashTowards(Vector3 dashTarget, bool success)
    {
        _pokemonPlayer.isBlockingPass = true;
        _pokemonPlayer.canBlock = false;

        float elapsed = 0f;
        Vector3 startPos = transform.position;

        while (elapsed < dashDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / dashDuration;
            transform.position = Vector3.Lerp(startPos, dashTarget, t);
            yield return null;
        }

        yield return new WaitForSeconds(0.3f);
        _pokemonPlayer.isBlockingPass = false;

        if (!success)
        {
            yield return new WaitForSeconds(timingToReblock);
        }

        _pokemonPlayer.canBlock = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pokemon"))
        {
            PokemonPlayer opponent = other.GetComponent<PokemonPlayer>();
            if (opponent != null && opponent.isPassing && _pokemonPlayer.isBlockingPass)
            {
                Debug.Log("Pass blocked");
                BasketBallManager.Instance.SetBallHolder(_pokemonPlayer);
            }
        }
    }
}