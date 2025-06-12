using System;
using UnityEngine;

[Serializable]
public class PokemonPlayerAnimator
{
    public SpriteRenderer pokemonSpriteRenderer;

    public float animationSpeed = 0.1f; // Temps entre chaque frame
    private float animationTimer = 0f;
    private int currentFrame = 0;

    private Sprite[] currentAnimation;

    public void HandleAnimation(Vector3 move, Pokemon actualPokemon)
    {
        if (move.magnitude == 0)
        {
            // Si le joueur ne bouge pas, afficher le sprite de base
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

        // Jouer l'animation
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
}