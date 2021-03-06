﻿using Morph.Julian;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Morph.Julian
{
    public class Bouncepad : MonoBehaviour
    {

        [SerializeField] private string m_playerTag;
        [SerializeField] private string m_animationTrigger;
        [SerializeField] private Animator m_animator;
        [SerializeField] private float m_bounceForce = 40f;
        [SerializeField] private bool m_bounceInPadDirection = true;
        [SerializeField] private Vector2 m_bouceDirection = Vector2.up;
        [SerializeField] private ForceMode2D m_forceMode = ForceMode2D.Impulse;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Rigidbody2D playerRigidbody = collision.collider.GetComponent<Rigidbody2D>();
            if (playerRigidbody != null && collision.collider.tag == m_playerTag)
            {
                playerRigidbody.AddForce((m_bounceInPadDirection ? (Vector2)transform.up : m_bouceDirection).normalized * m_bounceForce, m_forceMode);
                m_animator.SetTrigger(m_animationTrigger);
            }
        }
    }
}
