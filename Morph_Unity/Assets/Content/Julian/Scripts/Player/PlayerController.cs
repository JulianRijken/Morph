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
        private BoxCollider2D m_blobBoxCollider2D;
        private CircleCollider2D m_BallCircleCollider2D;
        private Animator m_animatior;

        [Header("Movement")]
        [SerializeField] private float m_strafeSpeed;
        [SerializeField] private float m_gravityForce;
        [SerializeField] private PlayerState m_playerState;
        [SerializeField] private float m_drag;
        private Vector2 m_velocity;
        private float m_gravity;
        private bool m_holdingAbility;

        private enum PlayerState {BlobGround, BlobAir, BlobBall, Bird }

        [Header("BallMode")]
        [SerializeField] private float m_ballGravity;
        [SerializeField] private float m_smashHeight;

        [Header("BirdMode")]
        [SerializeField] private float m_birdGravity;
        [SerializeField] private float m_flapHeight;
        [SerializeField] private float m_birdMoveSpeed;
        [SerializeField] private float m_becomeBirdHeight;
        [SerializeField] private float m_flapDelayTime;
        [SerializeField] private float m_birdAcceleration;
        private float m_birdVelocity;
        private int m_birdMoveSide;
        private bool m_canFlap = true;

        [Header("BlobMode")]
        [SerializeField] private float m_blobRotateGroundCheckDistance;

        [Header("Input")]
        private float m_strafeInput;

        [Header("Collision")]
        [SerializeField] private float m_groundCheckDistance;
        [SerializeField] private LayerMask m_groundLayer;
        [SerializeField] private float m_groundFether;
        [SerializeField] private float m_groundMaxAngle;
        private float m_groundHitDistance;
        private float m_groundHitAngle;
        private Vector2 m_groundOrgin { get => (Vector2)transform.position + m_blobBoxCollider2D.offset; }


        #region UnityFunctions

        private void Awake()
        {
            m_rigidbody2D = GetComponent<Rigidbody2D>();
            m_blobBoxCollider2D = GetComponent<BoxCollider2D>();
            m_BallCircleCollider2D = GetComponent<CircleCollider2D>();
            m_animatior = GetComponent<Animator>();
        }

        private void Start()
        {
            BecomeBlob();
            SwitchSide(1);
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

            if (context.performed)
            {

                if (m_strafeInput > 0)             
                    SwitchSide(1);              
                else if (m_strafeInput < 0)
                    SwitchSide(-1);
                
            }
        }
        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (OnGround())
                    DoJump();
                else if (m_playerState != PlayerState.Bird)
                    SwitchMode(PlayerState.Bird);
                else if (m_playerState == PlayerState.Bird)
                    DoFlap();
            }
        }
        public void OnAbility(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                m_holdingAbility = true;

                if (m_playerState != PlayerState.BlobBall)
                {
                    SwitchMode(PlayerState.BlobBall);
                    DoSmash();
                }
            }
            else if (context.canceled)
            {
                m_holdingAbility = false;

                if (m_playerState == PlayerState.BlobBall)
                {
                    if(m_groundHitDistance > m_becomeBirdHeight)
                        SwitchMode(PlayerState.Bird);
                    else
                        SwitchMode(PlayerState.BlobGround);
                }
            }

        }

        #endregion



        private void HandleSpriteRotation()
        {
            if (m_playerState == PlayerState.BlobGround)
            {
                Vector3 down = Vector2.down;

                Vector2 lefCastOrgin = (Vector2)transform.position + (Vector2.left * m_blobBoxCollider2D.size.x / 2f);
                RaycastHit2D leftHit = Physics2D.Raycast(lefCastOrgin, down, m_blobRotateGroundCheckDistance, m_groundLayer);

                Vector2 rightCastOrgin = (Vector2)transform.position + (Vector2.right * m_blobBoxCollider2D.size.x / 2f);
                RaycastHit2D rightHit = Physics2D.Raycast(rightCastOrgin, down, m_blobRotateGroundCheckDistance, m_groundLayer);

                Vector2 middleCastOrgin = (Vector2)transform.position;
                RaycastHit2D middleHit = Physics2D.Raycast(middleCastOrgin, down, m_blobRotateGroundCheckDistance, m_groundLayer);

                Quaternion toRot = Quaternion.identity;

                if (!(leftHit.collider == null || rightHit.collider == null || middleHit.collider == null))
                {
                    Vector2 normal = ((leftHit.normal / leftHit.distance) + (rightHit.normal / rightHit.distance) + (middleHit.normal / middleHit.distance)) / 3f;

                    Debug.DrawRay(transform.position, normal.normalized, Color.green);

                    toRot = Quaternion.LookRotation(Vector3.forward, normal.normalized);

                    Debug.DrawLine(lefCastOrgin, leftHit.point);
                    Debug.DrawLine(middleCastOrgin, middleHit.point);
                    Debug.DrawLine(rightCastOrgin, rightHit.point);
                }

                transform.rotation = Quaternion.Slerp(transform.rotation,toRot,Time.deltaTime * 7f);
            }
            else if(m_playerState == PlayerState.Bird)
            {
                Quaternion toRot = Quaternion.LookRotation(Vector3.forward, m_rigidbody2D.velocity.normalized);
                toRot *= Quaternion.Euler(0, 0, m_birdMoveSide > 0 ? 90f : -90f);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRot, Time.deltaTime * 7f);
            }

        }


        private void DoJump()
        {
            m_gravity = 10;
            SwitchMode(PlayerState.BlobAir);
        }

        private void DoFlap()
        {
            if (m_canFlap == true)
            {
                if (m_groundHitDistance > m_flapHeight)
                    m_gravity = 7;

                StartCoroutine(FlapDelay());
            }
        }

        private IEnumerator FlapDelay()
        {
            m_canFlap = false;
            yield return new WaitForSeconds(m_flapDelayTime);
            m_canFlap = true;
        }

        private void SwitchSide(int side)
        {
            side = Mathf.Clamp(side, -1, 1);
            Vector3 flipPos = m_spritesParent.localScale;
            flipPos.x = side;
            m_birdMoveSide = side;
            m_spritesParent.localScale = flipPos;
        }

        private void DoSmash()
        {
            if(m_groundHitDistance > m_smashHeight)
            m_rigidbody2D.AddForce(Vector2.down * 100, ForceMode2D.Impulse);     
        }

        private void BecomeBall()
        {
            //Debug.Log("Become Ball");
            m_blobBoxCollider2D.enabled = false;
            m_BallCircleCollider2D.enabled = true;
            m_rigidbody2D.constraints = RigidbodyConstraints2D.None;
            m_rigidbody2D.gravityScale = m_ballGravity;
        }

        private void BecomeBlob()
        {
            //Debug.Log("Become Normal");
            m_blobBoxCollider2D.enabled = true;
            m_BallCircleCollider2D.enabled = false;
            m_rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
            m_rigidbody2D.gravityScale = 0;
            StartCoroutine(RotateBack());
            m_velocity = m_rigidbody2D.velocity;
            m_gravity = 0f;
        }

        private void BecomeBird()
        {
            if (m_rigidbody2D.velocity.x > 0)
                SwitchSide(1);
            else if (m_rigidbody2D.velocity.x < 0)
                SwitchSide(-1);

            Debug.LogError("Zorg dat de bird ge fixed is en liniare naar beneden gaat maar wel kan flappen");

            //Debug.Log("Become Normal");
            m_blobBoxCollider2D.enabled = true;
            m_BallCircleCollider2D.enabled = false;
            m_rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
            m_rigidbody2D.gravityScale = 0;
            StartCoroutine(RotateBack());
            m_birdVelocity = m_rigidbody2D.velocity.x;
            m_velocity = m_rigidbody2D.velocity * Vector2.up;
            DoFlap();
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

            m_velocity = Vector2.MoveTowards(m_velocity, Vector2.zero, Time.fixedDeltaTime * m_drag);

            // Get strave input
            float strave = m_strafeInput;
            strave *= m_strafeSpeed;

            // Get the ground hit
            RaycastHit2D hit = GroundCheck();
            m_groundHitAngle = Vector2.Angle(hit.normal, Vector2.up);

            // Get distance
            m_groundHitDistance = m_groundOrgin.y - hit.point.y - (m_blobBoxCollider2D.size.y / 2f);

            if (m_playerState == PlayerState.BlobGround || m_playerState == PlayerState.BlobAir)
            {

                // Get Move Velocity
                Vector2 straveVeclocity = Vector2.zero;

                if (OnGround() && !OnMaxSlope())
                {
                    // IF on ground

                    // Apply gravity
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

            }
            else if(m_playerState == PlayerState.Bird)
            {

                m_gravity -= m_birdGravity * Time.fixedDeltaTime;

                m_birdVelocity = Mathf.MoveTowards(m_birdVelocity, m_birdMoveSpeed, Time.deltaTime * m_birdAcceleration);

                Vector2 move = m_velocity + (Vector2.right * (Mathf.Abs(m_birdVelocity) *  m_birdMoveSide)) + (Vector2.up * Mathf.Clamp(m_gravity,-6f,10f));

                m_rigidbody2D.velocity = move;
            }

        }

        private void SwitchMode(PlayerState toState)
        {
            if (toState == PlayerState.BlobGround || toState == PlayerState.BlobAir)
            {
                if (m_playerState == PlayerState.BlobBall)
                {
                    BecomeBlob();
                }
            }
            else if(toState == PlayerState.BlobBall)
            {
                BecomeBall();
            }
            else if (toState == PlayerState.Bird)
            {
                BecomeBird();
            }


            m_animatior.SetInteger("PlayerState", (int)toState);
            m_playerState = toState;
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
            return Physics2D.BoxCast(m_groundOrgin, m_blobBoxCollider2D.size, 0, Vector2.down, m_groundCheckDistance, m_groundLayer);
        }







    }
}
