using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Morph.Julian
{

    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(Animator))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Componenets")]
        [SerializeField] private Transform m_spritesParent;
        private Rigidbody2D m_rigidbody2D;
        private BoxCollider2D m_boxCollider2D;
        private Animator m_animatior;

        [Header("Movement")]
        [SerializeField] private float m_strafeSpeed;
        private Vector2 m_velocity;
        private float m_gravity;

        [Header("Input")]
        private float m_strafeInput;

        [Header("Collision")]
        [SerializeField] private float m_groundCheckDistance;
        [SerializeField] private LayerMask m_groundLayer;
        private float m_groundHitDistance;
        private Vector2 m_groundOrgin { get => (Vector2)transform.position + m_boxCollider2D.offset; }

        private void Awake()
        {
            m_rigidbody2D = GetComponent<Rigidbody2D>();
            m_boxCollider2D = GetComponent<BoxCollider2D>();
            m_animatior = GetComponent<Animator>();
        }

        private void FixedUpdate()
        {
            MovementUpdate();
        }


        #region Input
        public void OnStrafe(InputAction.CallbackContext context)
        {
            m_strafeInput = context.ReadValue<float>();

            Vector3 flipPos = m_spritesParent.localScale;
            if (m_strafeInput > 0)
                flipPos.x = 1;
            else if (m_strafeInput < 0)
                flipPos.x = -1;
            m_spritesParent.localScale = flipPos;
        }
        public void OnJump(InputAction.CallbackContext context)
        {
            //if (context.performed && m_jumpAllowed && !m_inJump && Settings.m_canJump)
            //    StartCoroutine(JumpCourutine());
        }
        #endregion


        private void MovementUpdate()
        {
            // Get the ground hit
            RaycastHit2D hit = GroundCheck();
            float angle = Vector2.Angle(hit.normal, Vector2.up);

            // Get distance
            m_groundHitDistance = m_groundOrgin.y - hit.point.y - (m_boxCollider2D.size.y / 2f);

            // Get strave input
            float strave = m_strafeInput;
            strave *= m_strafeSpeed;

            // Apply gravity
            m_gravity -= 9f * Time.fixedDeltaTime;

            Vector2 dirVelocity = (strave * (Quaternion.Euler(0, 0, -90f) * hit.normal));

            Vector2 move = m_velocity + dirVelocity + (Vector2.up * m_gravity);
            //m_rigidbody2D.velocity = move;
            m_rigidbody2D.MovePosition(m_rigidbody2D.position + move * Time.fixedDeltaTime);
            // ZORG DAT DE GRAVITY STOP ALS JE OP DE GROND STAAT 



#if UNITY_EDITOR
            // Debug
            Debug.Log(m_rigidbody2D.velocity.y);
            Debug.DrawLine(m_groundOrgin, hit.point,Color.red);
            Debug.DrawRay(transform.position,Vector2.right * strave);
            #endif
        }


        private void OnCollisionStay2D(Collision2D collision)
        {
            if (m_groundHitDistance < 0.1f)
                m_gravity = 0;
        }


        private RaycastHit2D GroundCheck()
        {
            return Physics2D.BoxCast(m_groundOrgin, m_boxCollider2D.size, 0, Vector2.down, m_groundCheckDistance, m_groundLayer);
        }
    }
}
