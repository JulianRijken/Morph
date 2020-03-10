using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator))]
public class MainMenu : MonoBehaviour
{
    [SerializeField] private PlayableDirector m_startGameDirector;
    [SerializeField] private PlayableDirector m_mainMenuDirector;

    private Animator m_animator;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();

        m_mainMenuDirector.Play();
    }

    public void PlayGame()
    {
        m_startGameDirector.Play();
        m_mainMenuDirector.Stop();
    }

    public void LoadMenu(int menu)
    {
        m_animator.SetInteger("menu", menu);
    }
}
