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
        [SerializeField] private float m_gravityForce;
        private Vector2 m_velocity;
        private float m_gravity;
        private bool m_inAir;

        [Header("Input")]
        private float m_strafeInput;

        [Header("Collision")]
        [SerializeField] private float m_groundCheckDistance;
        [SerializeField] private LayerMask m_groundLayer;
        [SerializeField] private float m_groundFether;
        [SerializeField] private float m_groundMaxAngle;
        private float m_groundHitDistance;
        private float m_groundHitAngle;
        private Vector2 m_groundOrgin { get => (Vector2)transform.position + m_boxCollider2D.offset; }

        #region UnityFunctions

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

        private void Update()
        {
            HandleSpriteRotation();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.contacts[0].normal.y < 0f)
            {
                m_gravity = 0;
            }
            else if (collision.contacts[0].normal.y > 0f)
            {
                m_inAir = false;
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
       
        }

        #endregion

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
            if (context.performed && OnGround())            
                DoJump();
            
        }
        public void OnAbility(InputAction.CallbackContext context)
        {
            if (context.performed && !OnGround() && !OnMaxSlope())
                DoSmash();

        }

        #endregion

        #region Functions

        private void HandleSpriteRotation()
        {
        }

        private void DoJump()
        {
            m_gravity = 10;
            m_inAir = true;
        }

        private void DoSmash()
        {
            Debug.Log("Do Smash");
            m_gravity -= 20;
            
        }

        private void MovementUpdate()
        {
            // Get the ground hit
            RaycastHit2D hit = GroundCheck();
            m_groundHitAngle = Vector2.Angle(hit.normal, Vector2.up);

            // Get distance
            m_groundHitDistance = m_groundOrgin.y - hit.point.y - (m_boxCollider2D.size.y / 2f);

            // Get strave input
            float strave = m_strafeInput;
            strave *= m_strafeSpeed;

            // Get Move Velocity
            Vector2 straveVeclocity = Vector2.zero;

            if (OnGround() && !OnMaxSlope())
            {
                // IF on ground
                m_gravity = 0f;
                straveVeclocity = (strave * (Quaternion.Euler(0, 0, -90f) * hit.normal));
            }
            else
            {
                // IF in air

                // Apply gravity
                m_gravity -= m_gravityForce * Time.fixedDeltaTime;
                straveVeclocity = Vector2.right * strave;
            }


    

            Vector2 move = m_velocity + straveVeclocity + (Vector2.up * m_gravity);
            m_rigidbody2D.velocity = move;
            //m_rigidbody2D.MovePosition(m_rigidbody2D.position + move * Time.fixedDeltaTime);
            // ZORG DAT DE GRAVITY STOP ALS JE OP DE GROND STAAT 



            #if UNITY_EDITOR
            // Debug
            // Debug.Log(m_rigidbody2D.velocity.y);
            // Debug.Log(OnMaxSlope());
            Debug.DrawLine(m_groundOrgin, hit.point,Color.red);
            Debug.DrawRay(transform.position,Vector2.right * strave);
            #endif
        }

        #endregion

        #region ReturnFuctions

        private bool OnGround()
        {
            return (m_groundHitDistance < m_groundFether && m_inAir == false) ? true : false;
        }

        private bool OnMaxSlope()
        {
            return (m_groundHitAngle > m_groundMaxAngle) ? true : false;
        }

        private RaycastHit2D GroundCheck()
        {
            return Physics2D.BoxCast(m_groundOrgin, m_boxCollider2D.size, 0, Vector2.down, m_groundCheckDistance, m_groundLayer);
        }

        #endregion

    }
}
