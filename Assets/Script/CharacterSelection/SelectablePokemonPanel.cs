using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SelectablePokemonPanel : MonoBehaviour
{
    public static SelectablePokemonPanel Instance;
    public GameObject selectablePokemonPrefab;
    
    public Pokemon[] selectedPlayer1Characters;
    public Pokemon[] selectedPlayer2Characters;
    
    public GameObject[] selectedPlayer1CharactersPreview;
    public GameObject[] selectedPlayer2CharactersPreview;
    
    public Button startButton;
    private int gameSceneIndex = 2;
    [NonSerialized] public int maxPoint = 21;

    public void Start()
    {
        Instance = this;

        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
        
        Pokemon[] pokemons = PokemonDatabase.Instance.pokemons;

        foreach (Pokemon pokemon in pokemons)
        {
            GameObject pokePrefab = Instantiate(selectablePokemonPrefab, this.transform);
            pokePrefab.name = "Selectable pokemon : " + pokemon.name;
            pokePrefab.GetComponentInChildren<SelectablePokemonPrefab>().Setup(pokemon, this);
        }
    }

    public void SetupCharacterSelectableFor2Players()
    {
        UpdateCharacterPreviews();
        CheckButtonState();
    }

    public void SetupCharacterSelectableFor1Player()
    {
        UpdateCharacterPreviews();
        CheckButtonState();
    }

    private void Update()
    {
        RefreshPreviews();

        if (Input.GetKeyUp(RemoteInput.B1))
        {
            RemoveLastSelectedPokemon(selectedPlayer1Characters);
        }

        if (Input.GetKeyUp(RemoteInput.B2))
        {
            RemoveLastSelectedPokemon(selectedPlayer2Characters);
        }
    }

    private void RemoveLastSelectedPokemon(Pokemon[] selectedCharacters)
    {
        for (int i = selectedCharacters.Length - 1; i >= 0; i--)
        {
            if (selectedCharacters[i] != null)
            {
                selectedCharacters[i] = null;
                break;
            }
        }
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
                        FadeSprite fadeSprite = previewObjects[i].GetComponent<FadeSprite>();
                        if (fadeSprite != null)
                        {
                            fadeSprite.Show(selectedCharacters[i]);
                        }
                    }
                    else
                    {
                        FadeSprite fadeSprite = previewObjects[i].GetComponent<FadeSprite>();
                        if (fadeSprite != null)
                        {
                            fadeSprite.Hide();
                        }
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

    public void LaunchGame()
    {
        if (EveryoneSelected())
        {
            SceneTransitor.Instance.LoadScene(gameSceneIndex, (gm, spp) =>
            {
                if (gm == null) Debug.LogWarning("Error getting game manager");
                if (selectedPlayer1Characters.Length == 3 && selectedPlayer2Characters.Length == 3)
                {
                    gm.StartMatch(
                        selectedPlayer1Characters.ToList(),
                        selectedPlayer2Characters.ToList(),
                        maxPoint
                    );
                }
                else
                {
                    Debug.LogWarning("Count of different selected characters isn't good : 3");
                    Debug.LogWarning("Length player 1 character : " + selectedPlayer1Characters.Length.ToString());
                    Debug.LogWarning("Length player 2 character : " + selectedPlayer2Characters.Length.ToString());
                }
            });
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

    public void ReturnFirstScene()
    {
        SceneTransitor.Instance.LoadScene(0);
    }
}