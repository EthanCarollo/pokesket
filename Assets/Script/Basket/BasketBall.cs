using System;
using System.Collections;
using UnityEngine;

public class BasketBall : MonoBehaviour
{ 
        private PokemonPlayer currentHolder => GameManager.Instance.currentBasketBallHolder;
                
        public Rigidbody rb;
        
        public void Update()
        {
                if (currentHolder != null)
                {
                        Vector3 offset = currentHolder.Direction * 0.5f;
                        transform.position = currentHolder.transform.position + offset;
                }
        }

        public void GoDirectlyIn(Vector3 target)
        {
                transform.parent = null;
                StartCoroutine(MoveToRim(target));
        }

        private IEnumerator MoveToRim(Vector3 target)
        {
                float duration = 0.5f;
                float time = 0f;
                Vector3 start = transform.position;

                while (time < duration)
                {
                        transform.position = Vector3.Lerp(start, target, time / duration);
                        time += Time.deltaTime;
                        yield return null;
                }

                transform.position = target;
        }

}