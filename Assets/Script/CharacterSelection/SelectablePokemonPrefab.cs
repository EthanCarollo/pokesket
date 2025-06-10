using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectablePokemonPrefab : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public Image pokemonImage;
    public GameObject selectedButtonIndicator;
    private Pokemon pokemon;
    private bool isSelected = false;
    
    public void Setup(Pokemon pokemon)
    {
        pokemonImage.sprite = pokemon.pokemonPortrait;
        this.pokemon = pokemon;
    }

    public void ShowSelectedButtonIndicator(bool showSelectedButtonIndicator)
    {
        selectedButtonIndicator.SetActive(showSelectedButtonIndicator);
        isSelected = showSelectedButtonIndicator;
    }

    public void OnSelect(BaseEventData eventData)
    {
        isSelected = true;
        ShowSelectedButtonIndicator(true);
        gameObject.SetActive(true);
        
        Debug.Log($"Pokemon {pokemon?.name} est sélectionné");
    }
    
    public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;
        ShowSelectedButtonIndicator(false);
        Debug.Log($"Pokemon {pokemon?.name} est désélectionné");
    }
}