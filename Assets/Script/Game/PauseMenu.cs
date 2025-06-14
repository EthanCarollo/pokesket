using System;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
        public GameObject pauseMenu;
        public bool IsPauseMenuOpen{ get { return pauseMenu.activeSelf; } }
        
        public void Start()
        {
                CloseMenu();
        }

        public void Update()
        {
                if ((Input.GetKeyDown(RemoteInput.START1) || Input.GetKeyDown(RemoteInput.START2)) && !IsPauseMenuOpen)
                {
                        OpenMenu();
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
}