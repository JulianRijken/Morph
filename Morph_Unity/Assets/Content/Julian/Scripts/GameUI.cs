using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Morph.Julian
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private GameObject m_mainMenu;
        [SerializeField] private GameObject m_pauseMenu;
        [SerializeField] private Button m_resumeButton;
        private Controls m_controls;
        private bool m_gamePaused;

        private void Awake()
        {
            m_controls = new Controls();
            m_controls.Game.Pause.performed += OnPause;

            m_mainMenu.SetActive(true);
            PauseGame(false);
        }

        private void Start()
        {
            Time.timeScale = 1;
        }

        private void OnPause(InputAction.CallbackContext context)
        {
            PauseGame(!m_gamePaused);
        }

        public void PauseGame(bool paused)
        {
            if (m_mainMenu.activeSelf == false)
            {
                m_gamePaused = paused;
                m_pauseMenu.SetActive(paused);

                if (paused)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    Time.timeScale = 0;
                    m_resumeButton.Select();
                }
                else
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    Time.timeScale = 1;
                }
            }
        }

        private void OnEnable()
        {
            m_controls.Enable();
        }

        private void OnDisable()
        {
            m_controls.Disable();
        }
    }
}