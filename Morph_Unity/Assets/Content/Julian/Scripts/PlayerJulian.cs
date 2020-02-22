using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Morph.Julian
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerJulian : MonoBehaviour
    {

        [Header("Collition")]
        [SerializeField] private float m_groundCheckDistance;
        [SerializeField] private LayerMask m_groundLayer;

        [Header("Movement")]
        [SerializeField] private float m_maxStrafeSpeed;
        [SerializeField] private float m_gravityForce;
        [SerializeField] private float m_accelerateSpeed;
        [SerializeField] private float m_deccelerateSpeed;
        [SerializeField] private float m_slopeLimit;
        [SerializeField] private float m_slopeSlowDownSpeed;
        [SerializeField] private float m_jumplength;
        [SerializeField] private float m_jumpStrength;
        [SerializeField] private AnimationCurve m_jumpCurve;

        private Rigidbody2D m_rigidbody2D;
        private BoxCollider2D m_boxCollider2D;

        private float m_strafeInput;
        private float m_smoothStrave;
        private float m_gravity;
        private float m_jumpForce;
        private bool m_inJump;
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
            m_gravity += m_gravityForce * Time.fixedDeltaTime;

            // if on ground
            if (hit.collider != null && hit.distance < 0.5f)
            {

                // if on slope
                if (angle > m_slopeLimit)
                {
                    if (hit.normal.x > 0)
                    {
                        m_strafeInput = Mathf.Clamp(m_strafeInput, 0, 1);
                        //m_strafeInput = 0;

                        m_slideVelocity -= (Vector2)(m_gravityForce * (Quaternion.Euler(0, 0, 90f) * hit.normal)) * Time.fixedDeltaTime;
                    }
                    else
                    {
                        m_strafeInput = Mathf.Clamp(m_strafeInput, -1, 0);
                        //m_strafeInput = 0;

                        m_slideVelocity += (Vector2)(m_gravityForce * (Quaternion.Euler(0, 0, 90f) * hit.normal)) * Time.fixedDeltaTime;
                    }
                }
                else
                {

                    // Set the player to the ground
                    if (hit.distance <= 0.05f)
                    {
                        if (m_jumpTime > 0.1f)
                            CancelJump();

                        m_gravity = 0;
                        m_slideVelocity = Vector2.Lerp(m_slideVelocity, Vector2.zero, Time.fixedDeltaTime * m_slopeSlowDownSpeed);
                    }
                }


                m_smoothStrave = Mathf.MoveTowards(m_smoothStrave, m_strafeInput * m_maxStrafeSpeed, Time.fixedDeltaTime * (m_strafeInput == 0 ? m_deccelerateSpeed : m_accelerateSpeed));


                //Debug.Log(moveVelocityX);
                //Debug.DrawLine(transform.position, (Vector2)transform.position + (Vector2.right * moveVelocityX), isAccelerating ? Color.red : Color.green);

                m_inputVelocity = (Vector2)(-m_smoothStrave * (Quaternion.Euler(0, 0, 90f) * hit.normal));

            }
            else // if in air
            {
                //m_smoothStrave = Mathf.Lerp(m_smoothStrave, 0, Time.deltaTime * 4f);
                m_smoothStrave = Mathf.MoveTowards(m_smoothStrave, m_strafeInput * m_maxStrafeSpeed, Time.fixedDeltaTime * 10f);

                m_inputVelocity.y = 0;
                m_inputVelocity.x = m_smoothStrave;
            }


            m_rigidbody2D.velocity = m_inputVelocity  + m_slideVelocity + (Vector2.down * (m_inJump ? m_jumpForce : m_gravity));

        }

        public void OnStrafe(InputAction.CallbackContext context)
        {
            m_strafeInput = context.ReadValue<float>();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if(context.performed)
                if(!m_inJump)
                    StartCoroutine(JumpCourutine());
        }


        private void CancelJump()
        {
            if (m_inJump)
            {
                StopCoroutine(JumpCourutine());
                m_inJump = false;
            }
        }

        private IEnumerator JumpCourutine()
        {
            m_inJump = true;
            m_jumpTime = 0f;

            while (m_jumpTime <= m_jumplength)
            {
                //Debug.Log(m_jumpTime);
                m_jumpForce = m_jumpCurve.Evaluate(m_jumpTime / m_jumplength) * m_jumpStrength;
                m_jumpTime += Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            m_inJump = false;
        }

        private RaycastHit2D GroundCheck()
        {     
           return Physics2D.BoxCast((Vector2)transform.position + m_boxCollider2D.offset, m_boxCollider2D.size, 0, Vector2.down, m_groundCheckDistance, m_groundLayer);
        }
    }
}
