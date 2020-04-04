using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator))]
public class MainMenu : MonoBehaviour
{
    [SerializeField] private PlayableDirector m_startGameDirector;
    [SerializeField] private PlayableDirector m_mainMenuDirector;

    private Animator m_animator;
    private Button m_buttonToSelect;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();

        m_mainMenuDirector.Play();
    }

    public void PlayGame()
    {
        if (m_animator.GetInteger("menu") == 0)
        {
            m_mainMenuDirector.Stop();
            m_startGameDirector.Play();
        }
    }

    public void QuitGame()
    {
        Application.Quit();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
        #endif
    }

    public void LoadMenu(int menu)
    {
        m_animator.SetInteger("menu", menu);
    }

}
