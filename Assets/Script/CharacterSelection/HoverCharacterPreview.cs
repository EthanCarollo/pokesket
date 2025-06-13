using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HoverCharacterPreview : MonoBehaviour
{
    public GameObject previewPanel;
    public Image pokemonPortrait;
    public Image pokemonTypeImage;
    public TextMeshProUGUI pokemonName;
    public Slider speedSlider;
    public Slider defenceSlider;
    public Slider shootSlider;
    public Slider passSlider;

    public void Start()
    {
        Hide();
    }

    public void Show(Pokemon pokemon)
    {
        previewPanel.SetActive(true);
        pokemonPortrait.sprite = pokemon.pokemonPortrait;
        pokemonName.text = pokemon.pokemonName;
        speedSlider.value = pokemon.speed;
        speedSlider.maxValue = 15;

        defenceSlider.value = pokemon.defence;
        defenceSlider.maxValue = 100;

        shootSlider.value = pokemon.shootPrecision;
        shootSlider.maxValue = 100;

        passSlider.value = pokemon.passPrecision;
        passSlider.maxValue = 100;

        pokemonTypeImage.sprite = pokemon.pokemonType.typeIcon;
    }

    public void Hide()
    {
        previewPanel.SetActive(false);
    }
}