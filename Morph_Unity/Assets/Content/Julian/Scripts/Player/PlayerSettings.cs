using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "ScriptableObjects/PlayerSettings", order = 1)]
public class PlayerSettings : ScriptableObject
{
    [Header("Movement Settings")]
    public float m_maxStrafeSpeed = 10f;
    public float m_gravityForce = 20f;
    public float m_accelerateSpeed = 60f;
    public float m_deccelerateSpeed = 110f;

    [Header("Sliding")]
    public float m_slideSpeed = 3.5f;
    public float m_slopeLimit = 50f;
    public float m_slopeSlowDownSpeed = 10f;

    [Header("Jumping")]
    public float m_slopeJumpCancelForce = 3f;
    public float m_jumplength = 1f;
    public float m_jumpHeight = 10f;
    public float m_jumpMidTime = 0.5f;
    public bool m_canJump = true;
    public AnimationCurve m_jumpCurve;
}
