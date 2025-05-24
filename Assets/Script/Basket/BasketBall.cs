using System;
using UnityEngine;

public class BasketBall : MonoBehaviour
{ 
        private PokemonPlayer currentHolder
        {
                get
                {
                        return GameManager.Instance.currentBasketBallHolder;
                }
        }
        
        public void Update()
        {
                if (currentHolder != null)
                {
                        Vector3 offset = currentHolder.Direction * 0.5f;
                        transform.position = currentHolder.transform.position + offset;
                }
        }

}