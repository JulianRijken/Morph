using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Break : MonoBehaviour
{
    [SerializeField] private float speedToBreak;
    [SerializeField] private List<Rigidbody2D> m_rigidbody2Ds = new List<Rigidbody2D>();
    [SerializeField] private PolygonCollider2D m_collider;

    private void Awake()
    {
        for (int i = 0; i < m_rigidbody2Ds.Count; i++)
            m_rigidbody2Ds[i].isKinematic = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Mathf.Abs(collision.attachedRigidbody.velocity.y) > speedToBreak && collision.gameObject.tag == "Player")
        {
            for (int i = 0; i < m_rigidbody2Ds.Count; i++)
                m_rigidbody2Ds[i].isKinematic = false;

            m_collider.enabled = false;
        }
    }
}
