using UnityEngine;

[CreateAssetMenu(fileName = "PokemonType", menuName = "Pokemon/PokemonType")]
public class PokemonType : ScriptableObject
{
    public string typeName;
    public GameObject particlePointPrefab;
    public Sprite typeIcon;
    // For HDR color
    [ColorUsage(true, true)]
    public Color typeColor;
    public Color noHdrTypeColor;

    public Texture trailTexture;
}