using System;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
        public GameObject pauseMenu;
        public bool IsPauseMenuOpen{ get { return pauseMenu.activeSelf; } }

        public Sprite defenseControlSprite;
        public Sprite attackControlSprite;
        public Image controlImage;
        
        public void Start()
        {
                CloseMenu();
        }

        public void Update()
        {
                if (Input.GetKeyDown(RemoteInput.START1) || Input.GetKeyDown(RemoteInput.START2))
                {
                        if (Input.GetButton("PauseMenu") && !IsPauseMenuOpen)
                        {
                                if (IsPauseMenuOpen)
                                {
                                        CloseMenu();
                                }
                                else
                                {
                                        OpenMenu();
                                
                                }
                        }
                }
        }

        public void OpenMenu()
        {
                pauseMenu.SetActive(true);
        }

        public void CloseMenu()
        {
                pauseMenu.SetActive(false);
        }

        public void SwapControl()
        {
                if (controlImage.sprite == defenseControlSprite)
                {
                        controlImage.sprite = attackControlSprite;
                }
                else
                {
                        controlImage.sprite = defenseControlSprite;
                }
        }
}