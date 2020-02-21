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
        [SerializeField] private float m_accelerateSpeed;
        [SerializeField] private float m_deccelerateSpeed;
        [SerializeField] private float m_slopeLimit;

        private Rigidbody2D m_rigidbody2D;
        private BoxCollider2D m_boxCollider2D;

        private float m_strafeInput;
        private float m_smoothStrave;
        private float m_gravity;
        private Vector2 m_slideVelocity;
        private Vector2 m_velocity;

        private void Start()
        {
            m_rigidbody2D = GetComponent<Rigidbody2D>();
            m_boxCollider2D = GetComponent<BoxCollider2D>();
        }

        public void OnStrafe(InputAction.CallbackContext context)
        {
            m_strafeInput = context.ReadValue<float>();
        }

        public void OnJump(InputAction.CallbackContext context)
        {

        }

        private void FixedUpdate()
        {
            RaycastHit2D hit = GroundCheck();
            float angle = Vector2.Angle(hit.normal,Vector2.up);
            m_gravity += 8 * Time.fixedDeltaTime;

            // if on ground
            if (hit.collider != null && hit.distance < 0.5f)
            {

                // if on slope
                if (angle > m_slopeLimit)
                {
                    if (hit.normal.x > 0)
                    {
                        //m_smoothStrave = Mathf.Clamp(m_smoothStrave, 0, 1 * m_maxStrafeSpeed);
                        //m_strafeInput = Mathf.Clamp(m_strafeInput, 0, 1);
                        m_strafeInput = 0;

                        m_slideVelocity -= (Vector2)(10 * (Quaternion.Euler(0, 0, 90f) * hit.normal)) * Time.fixedDeltaTime;
                    }
                    else
                    {
                        //m_smoothStrave = Mathf.Clamp(m_smoothStrave, -1 * m_maxStrafeSpeed, 0);
                        //m_strafeInput = Mathf.Clamp(m_strafeInput, -1, 0);
                        m_strafeInput = 0;

                        m_slideVelocity += (Vector2)(10 * (Quaternion.Euler(0, 0, 90f) * hit.normal)) * Time.fixedDeltaTime;
                    }
                }
                else
                {
                    // Set the player to the ground
                    if (hit.distance <= 0.05f)
                    {
                        m_gravity = 0;
                        m_slideVelocity = Vector2.MoveTowards(m_slideVelocity, Vector2.zero, Time.deltaTime * 5f);
                    }
                }


                m_smoothStrave = Mathf.MoveTowards(m_smoothStrave, m_strafeInput * m_maxStrafeSpeed, Time.fixedDeltaTime * (m_strafeInput == 0 ? m_deccelerateSpeed : m_accelerateSpeed));
                m_velocity = (Vector2)(-m_smoothStrave * (Quaternion.Euler(0, 0, 90f) * hit.normal));


            }
            else // if in air
            {
                m_velocity.y = 0;
                m_smoothStrave = Mathf.Lerp(m_smoothStrave, 0, Time.deltaTime * 5f);
                m_velocity.x = m_smoothStrave;
            }


            m_rigidbody2D.velocity = m_velocity + (Vector2.down * m_gravity) + m_slideVelocity;

        }


        private RaycastHit2D GroundCheck()
        {     
           return Physics2D.BoxCast((Vector2)transform.position + m_boxCollider2D.offset, m_boxCollider2D.size, 0, Vector2.down, m_groundCheckDistance, m_groundLayer);
        }
    }
}
