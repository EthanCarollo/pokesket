using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
        public static GameManager Instance;
        
        [SerializeField]
        private GameObject basketBallPrefab;
        [SerializeField]
        private GameObject pokePlayerPrefab;

        [NonSerialized]
        public BasketBall InstanciedBasketBall;
        [NonSerialized]
        public PokemonPlayer currentBasketBallHolder;
        
        public BasketRim basketRimRed;
        public BasketRim basketRimBlue;
        
        [NonSerialized]
        public PokemonPlayer playedPokePlayerBlue;
        [NonSerialized]
        public PokemonPlayer playedPokePlayerRed;

        public void Start()
        {
                Instance = this;
                StartMatch();
        }

        public void StartMatch()
        {
                // Put this in another method cause we could maybe call it from a SceneTransitor and not
                // on start of the whole script, shit is still temporary
                InstanciedBasketBall = Instantiate(basketBallPrefab).GetComponent<BasketBall>();
                playedPokePlayerBlue = Instantiate(pokePlayerPrefab).GetComponent<PokemonPlayer>();
                playedPokePlayerBlue.team = BasketTeam.Blue;
                // Temporary just had one player for test
                // playedPokePlayerRed = Instantiate(pokePlayerPrefab).GetComponent<PokemonPlayer>();
                // playedPokePlayerBlue.team = BasketTeam.Red;
        }
}

public enum BasketTeam
{
        Blue,
        Red
}