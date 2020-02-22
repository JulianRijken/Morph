using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Prallax : MonoBehaviour
{
    [SerializeField] private Transform m_followTransform;
    [SerializeField] private Vector2 m_offset;
    [SerializeField] private float m_distance;

    private void LateUpdate()
    {
        transform.position = (m_followTransform.position * m_distance) + (Vector3)m_offset;
    }
}
