using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Morph.Julian
{

    //===== PlayerController made by Julian Rijken ====\\
    // Todo fix: 
    // Head Collision for Jumping
    // Wierd bounce when jumping up against steep slopes
    // Input stopping when up against slopes without bouncing when running against slopes

    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {

        [Header("Collision")]
        [SerializeField] private float m_groundCheckDistance;
        [SerializeField] private LayerMask m_groundLayer;

        [Header("Movement")]
        [SerializeField] private PlayerSettings m_blobMovementSettings;
        [SerializeField] private PlayerSettings m_birdMovementSettings;

        private Rigidbody2D m_rigidbody2D;
        private BoxCollider2D m_boxCollider2D;
        private Animator m_animatior;

        private float m_strafeInput;
        private float m_smoothStrave;
        private float m_gravity;
        private float m_jumpForce;
        private bool m_inJump;
        private bool m_jumpAllowed;
        private float m_jumpTime = 0f;
        private Vector2 m_slideVelocity;
        private Vector2 m_inputVelocity;
        private Dictionary<PlayerType,PlayerSettings> m_playerSettings = new Dictionary<PlayerType, PlayerSettings>();
        private PlayerType m_activePlayerType;
        private enum PlayerType{ blob = 0, bird = 1 }
        private PlayerSettings Settings { get => m_playerSettings[m_activePlayerType]; }

        private void Start()
        {
            m_rigidbody2D = GetComponent<Rigidbody2D>();
            m_boxCollider2D = GetComponent<BoxCollider2D>();
            m_animatior = GetComponent<Animator>();

            m_playerSettings.Add(PlayerType.blob, m_blobMovementSettings);
            m_playerSettings.Add(PlayerType.bird, m_birdMovementSettings);

            SetPlayeType(PlayerType.blob);

            if (Settings.m_jumplength <= 0f)
                Settings.m_jumplength = 1f;
        }
        private void Update()
        {
            #if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Alpha1))
                SetPlayeType(PlayerType.blob);
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                SetPlayeType(PlayerType.bird);
            #endif
        }
        private void FixedUpdate()
        {
            HandlePlayerMovement();
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            SlowDownInput(collision);
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            SlowDownInput(collision);

            if (collision.contacts[0].normal.y < 0f)
                CancelJump(false);
        }


        public void OnStrafe(InputAction.CallbackContext context)
        {
            m_strafeInput = context.ReadValue<float>();
        }
        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed && m_jumpAllowed && !m_inJump && Settings.m_canJump)
                StartCoroutine(JumpCourutine());
        }


        private void HandlePlayerMovement()
        {
            RaycastHit2D hit = GroundCheck();
            float angle = Vector2.Angle(hit.normal, Vector2.up);

            bool isAccelerating = (m_smoothStrave > 0f && m_strafeInput > 0f) || (m_smoothStrave < 0f && m_strafeInput < 0f) ? true : false;

            m_gravity += Settings.m_gravityForce * Time.fixedDeltaTime;

            // if on ground
            if (hit.collider != null && hit.distance < 0.5f)
            {

                // if on slope
                if (angle > Settings.m_slopeLimit)
                {
                    m_jumpAllowed = false;

                    if (hit.normal.x > 0)
                    {
                        m_strafeInput = Mathf.Clamp(m_strafeInput, 0, 1);

                        if (m_strafeInput == 0)
                            m_slideVelocity -= (Vector2)(Quaternion.Euler(0, 0, 90f) * hit.normal) * Settings.m_slideSpeed * Time.fixedDeltaTime;

                    }
                    else
                    {
                        m_strafeInput = Mathf.Clamp(m_strafeInput, -1, 0);

                        if (m_strafeInput == 0)
                            m_slideVelocity += (Vector2)(Quaternion.Euler(0, 0, 90f) * hit.normal) * Settings.m_slideSpeed * Time.fixedDeltaTime;
                    }

                }
                else
                {
                    m_jumpAllowed = true;
                }

                // Set the player to the ground
                if (hit.distance <= 0.05f)
                {
                    m_gravity = 0;
                }


                // if on ground and falling
                if (m_jumpTime > Settings.m_jumpMidTime)
                    CancelJump(false);

                if (angle > 0 && m_jumpTime > 0.1f)
                    CancelJump(false, Settings.m_slopeJumpCancelForce);

                m_smoothStrave = Mathf.MoveTowards(m_smoothStrave, m_strafeInput * Settings.m_maxStrafeSpeed, Time.fixedDeltaTime * (isAccelerating ? Settings.m_accelerateSpeed : Settings.m_deccelerateSpeed));
                m_inputVelocity = (-m_smoothStrave * (Quaternion.Euler(0, 0, 90f) * hit.normal));

            }
            else // if in air
            {
                m_jumpAllowed = false;

                m_smoothStrave = Mathf.MoveTowards(m_smoothStrave, m_strafeInput * Settings.m_maxStrafeSpeed, Time.fixedDeltaTime * (isAccelerating ? Settings.m_accelerateSpeed : Settings.m_deccelerateSpeed));
                m_inputVelocity.x = m_smoothStrave;

                m_inputVelocity.y = 0;
            }


            m_slideVelocity = Vector2.MoveTowards(m_slideVelocity, Vector2.zero, Time.fixedDeltaTime * Settings.m_slopeSlowDownSpeed);

            m_rigidbody2D.velocity = m_inputVelocity + m_slideVelocity + (Vector2.down * (m_inJump ? m_jumpForce : m_gravity));


            Debug.DrawLine(transform.position, (Vector2)transform.position + Vector2.right * m_smoothStrave, isAccelerating ? Color.red : Color.green);
        }
        private void SlowDownInput(Collision2D collision)
        {
            if (collision.contactCount > 0)
            {
                //Debug.Log(Mathf.Abs(collision.contacts[0].normal.x));

                if (Mathf.Abs(collision.contacts[0].normal.x) >= 0.7f)
                    m_smoothStrave = 0f;
            }
        }
        private void CancelJump(bool keepForce = true, float extraForce = 0)
        {
            if (m_inJump == true)
            {
                if (keepForce)
                    m_gravity = m_jumpForce;
                else
                    m_gravity = 0;

                m_gravity += extraForce;

                m_inJump = false;
            }
        }


        private void SetPlayeType(PlayerType type) 
        {
            m_activePlayerType = type;
            m_animatior.SetInteger("playerType", (int)type);
            CancelJump(true);
        }

        private IEnumerator JumpCourutine()
        {
            m_inJump = true;
            m_jumpTime = 0f;

            while (m_jumpTime <= Settings.m_jumplength && m_inJump == true)
            {
                float time = m_jumpTime / Settings.m_jumplength;

                m_jumpForce = Mathf.Lerp(1, 0, Settings.m_jumpCurve.Evaluate(time)) * Settings.m_jumpHeight * (time <= Settings.m_jumpMidTime ? -1 : 1);
                m_jumpTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            CancelJump();
        }
        private RaycastHit2D GroundCheck()
        {
            return Physics2D.BoxCast((Vector2)transform.position + m_boxCollider2D.offset, m_boxCollider2D.size, 0, Vector2.down, m_groundCheckDistance, m_groundLayer);
        }

    }

}
