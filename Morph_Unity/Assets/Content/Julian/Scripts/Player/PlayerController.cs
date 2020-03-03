using System.Collections;
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

        [Header("Collition")]
        [SerializeField] private float m_groundCheckDistance;
        [SerializeField] private LayerMask m_groundLayer;

        [Header("Movement")]
        [SerializeField] private float m_maxStrafeSpeed;
        [SerializeField] private float m_gravityForce;
        [SerializeField] private float m_slideDrag;
        [SerializeField] private float m_accelerateSpeed;
        [SerializeField] private float m_deccelerateSpeed;
        [SerializeField] private float m_slopeLimit;
        [SerializeField] private float m_slopeSlowDownSpeed;
        [SerializeField] private float m_slopeJumpCancelForce;
        [SerializeField] private float m_jumplength;
        [SerializeField] private float m_jumpHeight;
        [SerializeField] private float m_jumpMidTime;
        [SerializeField] private AnimationCurve m_jumpCurve;

        private Rigidbody2D m_rigidbody2D;
        private BoxCollider2D m_boxCollider2D;

        private float m_strafeInput;
        private float m_smoothStrave;
        private float m_gravity;
        private float m_jumpForce;
        private bool m_inJump;
        private bool m_jumpAllowed;
        private float m_jumpTime = 0f;
        private Vector2 m_slideVelocity;
        private Vector2 m_inputVelocity;

        private void Start()
        {
            if (m_jumplength <= 0f)
                m_jumplength = 1f;

            m_rigidbody2D = GetComponent<Rigidbody2D>();
            m_boxCollider2D = GetComponent<BoxCollider2D>();
        }

        private void FixedUpdate()
        {
            RaycastHit2D hit = GroundCheck();
            float angle = Vector2.Angle(hit.normal,Vector2.up);

            bool isAccelerating = (m_smoothStrave > 0f && m_strafeInput > 0f) || (m_smoothStrave < 0f && m_strafeInput < 0f) ? true : false;

            m_gravity += m_gravityForce * Time.fixedDeltaTime;

            // if on ground
            if (hit.collider != null && hit.distance < 0.5f)
            {

                // if on slope
                if (angle > m_slopeLimit)
                {
                    m_jumpAllowed = false;

                    //if(m_inJump) CancelJump();

                    if (hit.normal.x > 0)
                    {
                        m_strafeInput = Mathf.Clamp(m_strafeInput, 0, 1);

                        if (m_strafeInput == 0)
                            m_slideVelocity -= (Vector2)((m_gravityForce / m_slideDrag) * (Quaternion.Euler(0, 0, 90f) * hit.normal)) * Time.fixedDeltaTime;
                    }
                    else
                    {
                        m_strafeInput = Mathf.Clamp(m_strafeInput, -1, 0);

                        if (m_strafeInput == 0)
                            m_slideVelocity += (Vector2)((m_gravityForce / m_slideDrag) * (Quaternion.Euler(0, 0, 90f) * hit.normal)) * Time.fixedDeltaTime;
                    }

                }
                else
                {
                    m_jumpAllowed = true;

                    // Set the player to the ground
                    if (hit.distance <= 0.05f)
                    {
                        m_gravity = 0;
                        m_slideVelocity = Vector2.Lerp(m_slideVelocity, Vector2.zero, Time.fixedDeltaTime * m_slopeSlowDownSpeed);
                    }
                }

                // if on ground and falling
                if (m_jumpTime > m_jumpMidTime)
                    CancelJump();

                if (angle > 0 && m_jumpTime > 0.1f)
                    CancelJump(false, m_slopeJumpCancelForce);

                m_smoothStrave = Mathf.MoveTowards(m_smoothStrave, m_strafeInput * m_maxStrafeSpeed, Time.fixedDeltaTime * (isAccelerating ? m_accelerateSpeed : m_deccelerateSpeed));
                m_inputVelocity = (-m_smoothStrave * (Quaternion.Euler(0, 0, 90f) * hit.normal));

            }
            else // if in air
            {
                m_jumpAllowed = false;

                m_smoothStrave = Mathf.MoveTowards(m_smoothStrave, m_strafeInput * m_maxStrafeSpeed, Time.fixedDeltaTime * (isAccelerating ? m_accelerateSpeed : m_deccelerateSpeed));
                m_inputVelocity.x = m_smoothStrave;

                m_inputVelocity.y = 0;
            }

            Debug.DrawLine(transform.position, (Vector2)transform.position + Vector2.right * m_smoothStrave, isAccelerating ? Color.red : Color.green);
            m_rigidbody2D.velocity = m_inputVelocity + m_slideVelocity + (Vector2.down * (m_inJump ? m_jumpForce : m_gravity));

        }

        public void OnStrafe(InputAction.CallbackContext context)
        {
            m_strafeInput = context.ReadValue<float>();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if(context.performed && m_jumpAllowed && !m_inJump)
                    StartCoroutine(JumpCourutine());
        }

        private void CancelJump(bool keepForce = true, float extraForce = 0)
        {
            if (m_inJump)
            {
                if (keepForce)
                    m_gravity = m_jumpForce;
                else
                    m_gravity = 0;

                m_gravity += extraForce;

                m_inJump = false;
            }
        }

        private IEnumerator JumpCourutine()
        {
            m_inJump = true;
            m_jumpTime = 0f;

            while (m_jumpTime <= m_jumplength && m_inJump == true)
            {
                float time = m_jumpTime / m_jumplength;

                m_jumpForce = Mathf.Lerp(1,0, m_jumpCurve.Evaluate(time)) * m_jumpHeight * (time <= m_jumpMidTime ? -1 : 1);
                m_jumpTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            CancelJump();
        }

        private RaycastHit2D GroundCheck()
        {     
           return Physics2D.BoxCast((Vector2)transform.position + m_boxCollider2D.offset, m_boxCollider2D.size, 0, Vector2.down, m_groundCheckDistance, m_groundLayer);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.contacts[0].normal.y < 0f)
                CancelJump(false);
        }
    }
}
