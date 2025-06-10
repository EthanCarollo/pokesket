using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectablePokemonPrefab : MonoBehaviour
{
    public Image pokemonImage;
    private Pokemon pokemon;
    SelectablePokemonPanel _selectablePokemonPanel;
    
    public void Setup(Pokemon pokemon, SelectablePokemonPanel selectablePokemonPanel)
    {
        pokemonImage.sprite = pokemon.pokemonPortrait;
        this.pokemon = pokemon;
        _selectablePokemonPanel = selectablePokemonPanel;
    }

    public void SelectPokemon(int playerIndex)
    {
        if (playerIndex == 0)
        {
            int slotToFill = _selectablePokemonPanel.selectedPlayer1Characters.ToList().FindAll(_pokemon => _pokemon != null).Count;
            _selectablePokemonPanel.SelectPokemonForPlayer1(slotToFill, pokemon);
        }
        else
        {
            int slotToFill = _selectablePokemonPanel.selectedPlayer2Characters.ToList().FindAll(_pokemon => _pokemon != null).Count;
            _selectablePokemonPanel.SelectPokemonForPlayer2(slotToFill, pokemon);
        }
    }

}