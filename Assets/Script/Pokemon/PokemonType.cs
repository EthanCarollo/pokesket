using UnityEngine;

[CreateAssetMenu(fileName = "PokemonType", menuName = "Pokemon/PokemonType")]
public class PokemonType : ScriptableObject
{
    public string typeName;
    public GameObject particlePointPrefab;
}