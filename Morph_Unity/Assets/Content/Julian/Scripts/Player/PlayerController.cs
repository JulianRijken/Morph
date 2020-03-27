using System.Collections;
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
        private CircleCollider2D m_circleCollider2D;
        private Animator m_animatior;

        [Header("Movement")]
        [SerializeField] private float m_strafeSpeed;
        [SerializeField] private float m_gravityForce;
        [SerializeField] private PlayerState m_playerState;
        [SerializeField] private float m_drag;
        private Vector2 m_velocity;
        private float m_gravity;
        private bool m_holdingAbility;

        private enum PlayerState {BlobGround, BlobAir, BlobBall, BirdAir }

        [Header("BallMode")]
        [SerializeField] private float m_ballGravity;

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
            m_circleCollider2D = GetComponent<CircleCollider2D>();
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
                if (m_playerState == PlayerState.BlobBall && m_holdingAbility)
                    return;

                SwitchMode(PlayerState.BlobGround);
                     
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
            if (context.performed)
            {
                m_holdingAbility = true;

                if (m_playerState != PlayerState.BlobBall)
                {
                    if (m_playerState == PlayerState.BlobGround)
                    {
                        SwitchMode(PlayerState.BlobBall);
                    }
                    else
                    {
                        SwitchMode(PlayerState.BlobBall);
                        DoSmash();
                    }
                }
            }
            else if (context.canceled)
            {
                m_holdingAbility = false;

                if (m_playerState == PlayerState.BlobBall)
                {
                    SwitchMode(PlayerState.BlobGround);
                }
            }

        }

        #endregion



        private void HandleSpriteRotation()
        {
        }

        private void DoJump()
        {
            m_gravity = 10;
            SwitchMode(PlayerState.BlobAir);
        }

        private void DoSmash()
        {
            m_rigidbody2D.AddForce(Vector2.down * 100, ForceMode2D.Impulse);     
        }

        private void BecomeBall()
        {
            Debug.Log("Become Ball");
            m_boxCollider2D.enabled = false;
            m_circleCollider2D.enabled = true;
            m_rigidbody2D.constraints = RigidbodyConstraints2D.None;
            m_rigidbody2D.gravityScale = m_ballGravity;
        }

        private void BecomeNormal()
        {
            Debug.Log("Become Normal");

            m_boxCollider2D.enabled = true;
            m_circleCollider2D.enabled = false;
            m_rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
            m_rigidbody2D.gravityScale = 0;
            StartCoroutine(RotateBack());
            m_velocity = m_rigidbody2D.velocity;
            m_gravity = 0f;
        }

        private IEnumerator RotateBack()
        {
            float time = 0;
            Quaternion oldRot = transform.rotation;
            while (time < 1f)
            {
                time += Time.deltaTime * 4f;
                transform.rotation = Quaternion.Slerp(oldRot, Quaternion.identity, time);
                yield return new WaitForEndOfFrame();
            }
        }

        private void MovementUpdate()
        {

            if (m_playerState == PlayerState.BlobGround || m_playerState == PlayerState.BlobAir)
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



                m_velocity = Vector2.MoveTowards(m_velocity, Vector2.zero, Time.fixedDeltaTime * m_drag);
                Vector2 move = m_velocity + straveVeclocity + (Vector2.up * m_gravity);
                m_rigidbody2D.velocity = move;
                //m_rigidbody2D.MovePosition(m_rigidbody2D.position + move * Time.fixedDeltaTime);
                // ZORG DAT DE GRAVITY STOP ALS JE OP DE GROND STAAT 

            }

        }

        private void SwitchMode(PlayerState state)
        {
            if (state == PlayerState.BlobGround || state == PlayerState.BlobAir)
            {
                if (m_playerState == PlayerState.BlobBall)
                {
                    BecomeNormal();
    
                }
            }
            else if(state == PlayerState.BlobBall)
            {
                BecomeBall();
            }


            m_animatior.SetInteger("PlayerState", (int)state);
            m_playerState = state;
        }

        private bool OnGround()
        {
            return (m_groundHitDistance < m_groundFether && m_playerState == PlayerState.BlobGround) ? true : false;
        }

        private bool OnMaxSlope()
        {
            return (m_groundHitAngle > m_groundMaxAngle) ? true : false;
        }

        private RaycastHit2D GroundCheck()
        {
            return Physics2D.BoxCast(m_groundOrgin, m_boxCollider2D.size, 0, Vector2.down, m_groundCheckDistance, m_groundLayer);
        }







    }
}
