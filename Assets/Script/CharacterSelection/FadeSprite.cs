using UnityEngine;

public class FadeSprite : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public float fadeSpeed = 5f;

    private float targetFade = 1f;
    private float currentFade = 0f;

    private bool isActive = false;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        spriteRenderer.material = new Material(spriteRenderer.material);   
    }

    private void Update()
    {
        if (spriteRenderer != null)
        {
            currentFade = Mathf.Lerp(currentFade, targetFade, Time.deltaTime * fadeSpeed);
            spriteRenderer.material.SetFloat("_Fade", currentFade);

            if (currentFade <= 0.01f && !isActive)
            {
                spriteRenderer.sprite = null; // On cache vraiment le sprite à la fin du fade-out
            }
        }
    }

    public void Show(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
        targetFade = 1f;
        isActive = true;
    }

    public void Hide()
    {
        targetFade = 0f;
        isActive = false;
    }
}
