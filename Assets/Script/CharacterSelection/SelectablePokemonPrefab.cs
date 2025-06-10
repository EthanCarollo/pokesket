using UnityEngine;
using UnityEngine.UI;

public class SelectablePokemonPrefab : MonoBehaviour
{
        public Image pokemonImage;
        private Pokemon _pokemon;
        
        public void Setup(Pokemon pokemon)
        {
                pokemonImage.sprite = pokemon.pokemonPortrait;
                _pokemon = pokemon;
        }

        public void SelectPokemon()
        {
                // Select Pokemon
        }
}