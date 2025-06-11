using UnityEngine;

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Pokemon")]
public class Pokemon : ScriptableObject
{
    public int id;
    public string pokemonName;
    public Sprite pokemonPortrait;
    public Sprite pokemonSprite;
    
    [Range(0, 15f)]
    public float speed;
    [Range(0, 100f)]
    public float defence;
    [Range(0, 100f)]
    public float shootPrecision;
    [Range(0, 100f)]
    public float passPrecision;
}