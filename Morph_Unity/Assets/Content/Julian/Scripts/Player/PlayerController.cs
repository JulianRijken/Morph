﻿using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Morph.Julian
{

    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(Animator))]
    public class PlayerController : MonoBehaviour
    {

        #region Variable

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
        private float m_lastRbVelocity;
        private float m_gravity;
        private bool m_holdingAbility;
        private enum PlayerState {BlobGround, BlobAir, BlobBall, Bird }

        [Header("BallMode")]
        [SerializeField] private float m_ballGravity;
        [SerializeField] private float m_smashHeight;
        [SerializeField] private float m_smashStrength;

        [Header("BirdMode")]
        [SerializeField] private float m_birdGravity;
        [SerializeField] private float m_birdMaxGravity;
        [SerializeField] private float m_birdFlapStrength;
        [SerializeField] private float m_birdStartFlapStrength;
        [SerializeField] private float m_birdFlapHeight;
        [SerializeField] private float m_birdMoveSpeed;
        [SerializeField] private float m_birdTransformHeight;
        [SerializeField] private float m_birdTransformSpeed;
        [SerializeField] private float m_birdFlapDelayTime;
        [SerializeField] private float m_birdAcceleration;
        [SerializeField] private float m_birdRotateSpeed;
        private float m_birdVelocity;
        private int m_birdMoveSide;
        private bool m_birdCanFlap = true;

        [Header("BlobMode")]
        [SerializeField] private float m_blobRotateGroundCheckDistance;
        [SerializeField] private float m_blobRotateSpeed;

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

        [Header("Animation")]
        [SerializeField] private string m_playerStateKey;
        [SerializeField] private string m_flapTriggerKey;
        [SerializeField] private string m_straveInputKey;
        [SerializeField] private string m_jumpKey;
        [SerializeField] private string m_velocityKey;

        #endregion


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
            HandleMovement();
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


        #endregion


        #region Input
        public void OnStrafe(InputAction.CallbackContext context)
        {
            m_strafeInput = context.ReadValue<float>();
            m_animatior.SetFloat(m_straveInputKey, Mathf.Abs(m_strafeInput));

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
                    if(m_groundHitDistance > m_birdTransformHeight && m_rigidbody2D.velocity.magnitude > m_birdTransformSpeed)
                        SwitchMode(PlayerState.Bird);
                    else
                        SwitchMode(PlayerState.BlobGround);
                }
            }

        }

        #endregion


        #region GeneralFunctions

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

                transform.rotation = Quaternion.Slerp(transform.rotation,toRot,Time.fixedDeltaTime * m_blobRotateSpeed);
            }
            else if(m_playerState == PlayerState.Bird)
            {
                Quaternion toRot = Quaternion.LookRotation(Vector3.forward, m_rigidbody2D.velocity.normalized);
                toRot *= Quaternion.Euler(0, 0, m_birdMoveSide > 0 ? 90f : -90f);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRot, Time.fixedDeltaTime * m_birdRotateSpeed);
            }
            else if(m_playerState == PlayerState.BlobAir)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, Time.fixedDeltaTime * m_blobRotateSpeed);
            }


        }

        private void HandleMovement()
        {
            
            m_animatior.SetFloat(m_velocityKey,Mathf.Abs(m_lastRbVelocity - m_rigidbody2D.velocity.magnitude));
            m_lastRbVelocity = m_rigidbody2D.velocity.magnitude;

            m_velocity = Vector2.MoveTowards(m_velocity, Vector2.zero, Time.fixedDeltaTime * m_drag);

            // Get strave input
            float strave = m_strafeInput;
            strave *= m_strafeSpeed;

            // Get the ground hit
            RaycastHit2D hit = GetGroundCheck();
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

                m_birdVelocity = Mathf.MoveTowards(m_birdVelocity, m_birdMoveSpeed, Time.fixedDeltaTime * m_birdAcceleration);

                Vector2 move = m_velocity + (Vector2.right * (Mathf.Abs(m_birdVelocity) *  m_birdMoveSide)) + (Vector2.up * Mathf.Clamp(m_gravity,m_birdMaxGravity,100f));

                m_rigidbody2D.velocity = move;
            }

        }


        private void DoJump()
        {
            m_animatior.SetTrigger(m_jumpKey);
            m_gravity = 10;
            SwitchMode(PlayerState.BlobAir);
            RotateBack();
        }

        private void DoFlap()
        {
            if (m_birdCanFlap == true && m_groundHitDistance > m_birdFlapHeight)
            {
                m_gravity = m_birdFlapStrength;
                m_animatior.SetTrigger(m_flapTriggerKey);
                StartCoroutine(FlapDelay());
            }
        }

        private void DoSmash()
        {
            if (m_groundHitDistance > m_smashHeight)
            {
                m_rigidbody2D.AddForce(Vector2.down * m_smashStrength, ForceMode2D.Impulse);
            }
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


            //Debug.Log("Become Normal");
            m_blobBoxCollider2D.enabled = true;
            m_BallCircleCollider2D.enabled = false;
            m_rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
            m_rigidbody2D.gravityScale = 0;
            StartCoroutine(RotateBack());
            m_birdVelocity = Mathf.Abs(m_rigidbody2D.velocity.x);
            m_velocity = (m_rigidbody2D.velocity.y) > 0 ? (m_rigidbody2D.velocity * Vector2.up) : Vector2.zero;
            DoFlap();
        }


        private void SwitchSide(int side)
        {
            side = Mathf.Clamp(side, -1, 1);
            Vector3 flipPos = m_spritesParent.localScale;
            flipPos.x = side;
            m_birdMoveSide = side;
            m_spritesParent.localScale = flipPos;
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


            m_animatior.SetInteger(m_playerStateKey, (int)toState);
            m_playerState = toState;
        }


        private IEnumerator FlapDelay()
        {
            m_birdCanFlap = false;
            yield return new WaitForSeconds(m_birdFlapDelayTime);
            m_birdCanFlap = true;
        }

        private IEnumerator RotateBack()
        {
            float time = 0;
            Quaternion oldRot = transform.rotation;
            while (time < 1f)
            {
                time += Time.fixedDeltaTime * 4f;
                transform.rotation = Quaternion.Slerp(oldRot, Quaternion.identity, time);
                yield return new WaitForFixedUpdate();
            }
        }

        #endregion


        #region ReturnFunctions

        private bool OnGround()
        {
            return (m_groundHitDistance < m_groundFether && m_playerState == PlayerState.BlobGround) ? true : false;
        }

        private bool OnMaxSlope()
        {
            return (m_groundHitAngle > m_groundMaxAngle) ? true : false;
        }

        private RaycastHit2D GetGroundCheck()
        {
            return Physics2D.BoxCast(m_groundOrgin, m_blobBoxCollider2D.size, 0, Vector2.down, m_groundCheckDistance, m_groundLayer);
        }

        #endregion

    }

}
