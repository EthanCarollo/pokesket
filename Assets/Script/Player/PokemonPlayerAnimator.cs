using System;
using UnityEngine;

[Serializable]
public class PokemonPlayerAnimator
{
    public SpriteRenderer pokemonSpriteRenderer;

    public float animationSpeed = 0.1f;
    private float animationTimer = 0f;
    private int currentFrame = 0;

    private Sprite[] currentAnimation;

    // Animation temporaire (One-shot)
    private Sprite[] oneShotAnimation = null;
    private Action onOneShotComplete;

    public void PlayOneShotAnimation(Sprite[] animation, Action onComplete = null)
    {
        if (animation == null || animation.Length == 0)
        {
            Debug.LogWarning("Animation is null or empty.");
            return;
        }

        oneShotAnimation = animation;
        onOneShotComplete = onComplete;
        animationTimer = 0f;
        currentFrame = 0;
    }

    public void HandleAnimation(Vector3 move, Pokemon actualPokemon)
    {
        // Si une animation temporaire est en cours, on la joue en priorité
        if (oneShotAnimation != null)
        {
            AnimateOneShot();
            return;
        }

        if (move.magnitude == 0)
        {
            pokemonSpriteRenderer.sprite = actualPokemon.pokemonSprite;
            animationTimer = 0f;
            currentFrame = 0;
            return;
        }

        // Sélectionner l'animation en fonction de la direction
        if (move.x > 0)
        {
            currentAnimation = move.z > 0 ? actualPokemon.topRightAnimation : actualPokemon.bottomRightAnimation;
        }
        else if (move.x < 0)
        {
            currentAnimation = move.z > 0 ? actualPokemon.topLeftAnimation : actualPokemon.bottomLeftAnimation;
        }
        else
        {
            if (move.z > 0)
            {
                currentAnimation = actualPokemon.topLeftAnimation;
            }
            else if (move.z < 0)
            {
                currentAnimation = actualPokemon.bottomLeftAnimation;
            }
        }

        AnimateCurrentSprite();
    }

    private void AnimateCurrentSprite()
    {
        animationTimer += Time.deltaTime;

        if (animationTimer >= animationSpeed)
        {
            animationTimer = 0f;
            currentFrame = (currentFrame + 1) % currentAnimation.Length;
        }

        pokemonSpriteRenderer.sprite = currentAnimation[currentFrame];
    }

    private void AnimateOneShot()
    {
        animationTimer += Time.deltaTime;

        if (animationTimer >= animationSpeed)
        {
            animationTimer = 0f;
            currentFrame++;

            if (currentFrame >= oneShotAnimation.Length)
            {
                // Fin de l'animation temporaire
                oneShotAnimation = null;
                onOneShotComplete?.Invoke();
                return;
            }
        }

        pokemonSpriteRenderer.sprite = oneShotAnimation[currentFrame];
    }
}
