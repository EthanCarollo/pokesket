using UnityEngine;

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Pokemon")]
public class Pokemon : ScriptableObject
{
    public int id;
    public string pokemonName;
    public Sprite pokemonPortrait;
    public Sprite pokemonSprite;
    public PokemonType pokemonType;
    
    [Range(0, 15f)]
    public float speed;
    [Range(0, 100f)]
    public float defence;
    [Range(0, 100f)]
    public float shootPrecision;
    [Range(0, 100f)]
    public float passPrecision;

    [Header("Movement")]
    public Sprite[] bottomRightAnimation;
    public Sprite[] bottomLeftAnimation;
    public Sprite[] topRightAnimation;
    public Sprite[] topLeftAnimation;
    
    [Header("Shoot")]
    public Sprite[] shootTopLeftAnimation;
    public Sprite[] shootTopRightAnimation;
    public Sprite[] shootBottomRightAnimation;
    public Sprite[] shootBottomLeftAnimation;
}