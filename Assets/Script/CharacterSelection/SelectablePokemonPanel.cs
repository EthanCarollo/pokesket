using System;
using UnityEngine;

public class SelectablePokemonPanel : MonoBehaviour
{
    public GameObject selectablePokemonPrefab;
    
    // Selected player Characters prefab
    public Pokemon[] selectedPlayer1Characters;
    public Pokemon[] selectedPlayer2Characters;
    
    // Player Characters preview prefab
    public GameObject[] selectedPlayer1CharactersPreview;
    public GameObject[] selectedPlayer2CharactersPreview;

    public void Start()
    {
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
        
        
        Pokemon[] pokemons = PokemonDatabase.Instance.pokemons;

        foreach (Pokemon pokemon in pokemons)
        {
            GameObject pokePrefab = Instantiate(selectablePokemonPrefab, this.transform);
            pokePrefab.name = "Selectable pokemon : " + pokemon.name;
            pokePrefab.GetComponent<SelectablePokemonPrefab>().Setup(pokemon);
        }
        
        UpdateCharacterPreviews();
    }

    private void UpdateCharacterPreviews()
    {
        UpdatePlayerPreviews(selectedPlayer1Characters, selectedPlayer1CharactersPreview);
        UpdatePlayerPreviews(selectedPlayer2Characters, selectedPlayer2CharactersPreview);
    }
    
    private void UpdatePlayerPreviews(Pokemon[] selectedCharacters, GameObject[] previewObjects)
    {
        for (int i = 0; i < previewObjects.Length; i++)
        {
            if (previewObjects[i] != null)
            {
                SpriteRenderer spriteRenderer = previewObjects[i].GetComponent<SpriteRenderer>();
                
                if (spriteRenderer != null)
                {
                    if (i < selectedCharacters.Length && selectedCharacters[i] != null)
                    {
                        spriteRenderer.sprite = selectedCharacters[i].pokemonSprite;
                        spriteRenderer.enabled = true;
                    }
                    else
                    {
                        spriteRenderer.sprite = null;
                        spriteRenderer.enabled = false;
                    }
                }
            }
        }
    }
    
    public void RefreshPreviews()
    {
        UpdateCharacterPreviews();
    }
    
    public void SelectPokemonForPlayer1(int slot, Pokemon pokemon)
    {
        if (slot >= 0 && slot < selectedPlayer1Characters.Length)
        {
            selectedPlayer1Characters[slot] = pokemon;
            UpdateCharacterPreviews();
        }
    }
    
    public void SelectPokemonForPlayer2(int slot, Pokemon pokemon)
    {
        if (slot >= 0 && slot < selectedPlayer2Characters.Length)
        {
            selectedPlayer2Characters[slot] = pokemon;
            UpdateCharacterPreviews();
        }
    }
}