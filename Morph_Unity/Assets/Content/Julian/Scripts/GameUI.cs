using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameUI : MonoBehaviour
{
    [SerializeField] private GameObject m_mainMenu;
    [SerializeField] private GameObject m_pauseMenu;
    // Add the pause menu en main menu en zet pause aan en uit
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
                Time.timeScale = 0;
            }
            else
            {
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
