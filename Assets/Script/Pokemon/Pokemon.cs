using UnityEngine;

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Pokemon")]
public class Pokemon : ScriptableObject
{
    public int id;
    public string pokemonName;
    public float speed;
    public Sprite pokemonPortrait;
    public Sprite image;
}