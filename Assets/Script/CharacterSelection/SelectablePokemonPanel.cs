using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectablePokemonPanel : MonoBehaviour
{
    public GameObject selectablePokemonPrefab;
    
    public Pokemon[] selectedPlayer1Characters;
    public Pokemon[] selectedPlayer2Characters;
    
    public GameObject[] selectedPlayer1CharactersPreview;
    public GameObject[] selectedPlayer2CharactersPreview;
    
    public Button startButton;
    private int gameSceneIndex = 2;
    
    private Button firstPokemonButton; // Référence au premier bouton créé

    public void Start()
    {
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
        
        Pokemon[] pokemons = PokemonDatabase.Instance.pokemons;
        bool isFirstPokemon = true;

        foreach (Pokemon pokemon in pokemons)
        {
            GameObject pokePrefab = Instantiate(selectablePokemonPrefab, this.transform);
            pokePrefab.name = "Selectable pokemon : " + pokemon.name;
            pokePrefab.GetComponentInChildren<SelectablePokemonPrefab>().Setup(pokemon);
            
            // Garder une référence au premier bouton créé
            if (isFirstPokemon)
            {
                firstPokemonButton = pokePrefab.GetComponentInChildren<Button>();
                isFirstPokemon = false;
            }
        }
        
        UpdateCharacterPreviews();
        CheckButtonState();
        
        // Select the first pokemon for a better UX
        StartCoroutine(SelectFirstPokemonAfterFrame());
    }
    
    private IEnumerator SelectFirstPokemonAfterFrame()
    {
        yield return new WaitForEndOfFrame();
        
        if (firstPokemonButton != null)
        {
            EventSystem eventSystem = EventSystem.current;
            if (eventSystem != null)
            {
                eventSystem.SetSelectedGameObject(firstPokemonButton.gameObject);
            }
        }
    }

    private void Update()
    {
        RefreshPreviews();
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
    
    private void CheckButtonState()
    {
        if (startButton != null)
            startButton.interactable = EveryoneSelected();
    }

    public bool EveryoneSelected()
    {
        bool allSelected = true;
        
        if(selectedPlayer1Characters.Length < 3)
        {
            allSelected = false;
        }
        foreach (Pokemon pokemon in selectedPlayer1Characters)
        {
            if (pokemon == null) allSelected = false;
        }
        
        
        if(selectedPlayer2Characters.Length < 3)
        {
            allSelected = false;
        }
        foreach (Pokemon pokemon in selectedPlayer2Characters)
        {
            if (pokemon == null) allSelected = false;
        }
        return allSelected;
    }
    
    public void RefreshPreviews()
    {
        UpdateCharacterPreviews();
        CheckButtonState();
    }
    
    public void LaunchGameTest()
    {
        SceneTransitor.Instance.LoadScene(gameSceneIndex);
    }

    public void LaunchGame()
    {
        if (EveryoneSelected())
        {
            SceneTransitor.Instance.LoadScene(gameSceneIndex);
        }
    }
    
    public void SelectPokemonForPlayer1(int slot, Pokemon pokemon)
    {
        if (slot >= 0 && slot < selectedPlayer1Characters.Length)
        {
            selectedPlayer1Characters[slot] = pokemon;
            UpdateCharacterPreviews();
            CheckButtonState();
        }
    }
    
    public void SelectPokemonForPlayer2(int slot, Pokemon pokemon)
    {
        if (slot >= 0 && slot < selectedPlayer2Characters.Length)
        {
            selectedPlayer2Characters[slot] = pokemon;
            UpdateCharacterPreviews();
            CheckButtonState();
        }
    }
}