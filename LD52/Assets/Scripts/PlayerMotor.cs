using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace LD52
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMotor : MonoBehaviour
    {
        private static readonly int s_Speed = Animator.StringToHash("Speed");
        private static readonly int s_VSpeed = Animator.StringToHash("vSpeed");
        private static readonly int s_IsGrounded = Animator.StringToHash("IsGrounded");
        private static readonly int s_IsDashing = Animator.StringToHash("IsDashing");
        private static readonly int s_IsTouchingWall = Animator.StringToHash("IsTouchingWall");
        
        public bool FacingRight { get; private set; } = true;

        [Header("Movement")]
        [SerializeField] private float m_MoveSpeed = 10f;
        [SerializeField] private float m_Acceleration = 7f;
        [SerializeField] private float m_Deceleration = 7f;
        [SerializeField] private float m_VelocityPower = 0.9f;

        [Space]
        [SerializeField] private float m_FrictionAmount = 0.2f;

        [Header("Jump")]
        [SerializeField] private float m_JumpForce = 80.0f;
        [SerializeField] private float m_JumpCutMultiplier = 0.5f;

        [Space]
        [SerializeField] private float m_JumpCoyoteTime = 0.1f;
        [SerializeField] private float m_JumpBufferTime = 0.1f;

        [Space]
        [SerializeField] private float m_GravityScale = 1f;
        [SerializeField] private float m_FallGravityMultiplier = 2f;

        [Header("Wall Jump")]
        [SerializeField] private float m_WallSlidingSpeed = 5f;
        [SerializeField] private Vector2 m_WallForce = new Vector2(20f, 15f);
        [SerializeField] private float m_WallJumpTime = 0.1f;

        [Header("Dash")]
        [SerializeField] private float m_DashForce = 50f;
        [SerializeField] private float m_DashTime = 0.1f;
        [SerializeField] private float m_NextDashTime = 0.1f;

        [Header("Checks")]
        [SerializeField] private Transform m_GroundCheck;
        [SerializeField] private Vector2 m_GroundCheckSize = new Vector2(0.5f, 0.02f);
        [SerializeField] private LayerMask m_GroundLayer;

        [Space]
        [SerializeField] private Transform m_WallCheck;
        [SerializeField] private float m_WallCheckRadius = 0.3f;
        [SerializeField] private LayerMask m_WallLayer;

        // Jump
        private bool m_IsJumping;
        private float m_LastJumpTime;
        private bool m_JumpInputReleased;

        // Wall Jump
        private bool m_WallJumping;

        // Dash
        private bool m_IsDashing;
        private bool m_CanDash = true;
        private bool m_IsDashTime;

        // Ground Check
        private bool m_IsGrounded;
        private bool m_WasGrounded;
        private float m_LastGroundedTime;

        // Wall Check
        private bool m_IsTouchingWall;
        private bool m_IsSliding;

        // Flip

        private Rigidbody2D m_Rigidbody;
        private Animator m_Animator;
        private PlayerCombat m_PlayerCombat;
        private static readonly int s_YSpeed = Animator.StringToHash("ySpeed");

        private void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody2D>();
            m_Animator = GetComponentInChildren<Animator>();
            m_PlayerCombat = GetComponent<PlayerCombat>();
        }

        private void FixedUpdate()
        {
            m_WasGrounded = m_IsGrounded;

            if (Physics2D.OverlapBox(m_GroundCheck.position, m_GroundCheckSize, 0.0f, m_GroundLayer))
            {
                m_IsGrounded = true;
                m_LastGroundedTime = m_JumpCoyoteTime;
            }
            else m_IsGrounded = false;

            m_IsTouchingWall = Physics2D.OverlapCircle(m_WallCheck.position, m_WallCheckRadius, m_WallLayer);
        }

        public void Move(float move)
        {
            // Movement
            float targetSpeed = move * m_MoveSpeed;
            float speedDiff = targetSpeed - m_Rigidbody.velocity.x;
            float acelRate = Mathf.Abs(targetSpeed) > 0.01f ? m_Acceleration : m_Deceleration;
            float movement = Mathf.Pow(Mathf.Abs(speedDiff) * acelRate, m_VelocityPower) * Mathf.Sign(speedDiff);

            if (!m_IsDashing && !m_PlayerCombat.IsAttacking)
                m_Rigidbody.AddForce(movement * Vector2.right);

            // Friction
            if (Mathf.Abs(move) > 0.01f)
            {
                float amount = Mathf.Min(Mathf.Abs(m_Rigidbody.velocity.x), Mathf.Abs(m_FrictionAmount));
                amount *= Mathf.Sign(m_Rigidbody.velocity.x);
                m_Rigidbody.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
            }

            // Jump
            if (m_JumpInputReleased)
                m_IsJumping = false;

            if (m_LastGroundedTime > 0.0f && m_LastJumpTime > 0.0f && !m_IsJumping)
                Jump();

            m_LastGroundedTime -= Time.deltaTime;
            m_LastJumpTime -= Time.deltaTime;

            // Wall Jumping
            m_IsSliding = m_IsTouchingWall && !m_IsGrounded && move != 0.0f;

            if (m_IsSliding)
            {
                Vector2 rbv = m_Rigidbody.velocity;
                m_Rigidbody.velocity = new Vector2(rbv.x, Mathf.Clamp(rbv.y, -m_WallSlidingSpeed, float.MaxValue));
            }

            if (m_WallJumping)
                WallJump();

            // Dashing
            if (m_IsDashing)
                Dash();

            if ((m_IsGrounded || m_IsTouchingWall) && m_IsDashTime)
                m_CanDash = true;

            // Gravity Scaling
            if (!m_IsDashing)
            {
                if (m_Rigidbody.velocity.y < 0.0f)
                    m_Rigidbody.gravityScale = m_GravityScale * m_FallGravityMultiplier;
                else m_Rigidbody.gravityScale = m_GravityScale;
            }

            // Flipping the player
            if (m_PlayerCombat.IsAttacking)
                return;
            
            switch (FacingRight)
            {
                case true when move < 0.0f:
                case false when move > 0.0f:
                    Flip();
                    break;
            }
        }

        #region Jump

        public void OnJump(InputAction.CallbackContext context)
        {
            m_LastJumpTime = m_JumpBufferTime;
        }

        private void Jump()
        {
            m_Rigidbody.AddForce(Vector2.up * m_JumpForce, ForceMode2D.Impulse);
            m_LastGroundedTime = 0.0f;
            m_LastJumpTime = 0.0f;
            m_IsJumping = true;
            m_JumpInputReleased = false;
        }

        public void OnJumpStop(InputAction.CallbackContext context)
        {
            if (m_Rigidbody.velocity.y > 0.0f && m_IsJumping)
                m_Rigidbody.AddForce((1 - m_JumpCutMultiplier) * m_Rigidbody.velocity.y * Vector2.down, ForceMode2D.Impulse);

            m_JumpInputReleased = true;
            m_LastJumpTime = 0.0f;
        }

        #endregion

        #region Wall Jump

        public void OnWallJump(InputAction.CallbackContext context)
        {
            if (!m_IsSliding) 
                return;
            
            m_WallJumping = true;
            Invoke(nameof(OnWallJumpStop), m_WallJumpTime);
        }

        private void WallJump()
        {
            m_Rigidbody.velocity = new Vector2(m_WallForce.x * (FacingRight ? -1 : 1), m_WallForce.y);
        }

        private void OnWallJumpStop()
        {
            m_WallJumping = false;
        }

        #endregion

        #region Dash

        public void OnDash(InputAction.CallbackContext context)
        {
            if (m_IsDashing || !m_CanDash)
                return;
            
            m_IsDashing = true;
            m_CanDash = false;
            m_IsDashTime = false;
            m_Rigidbody.gravityScale = 0;
            m_Rigidbody.velocity = Vector2.zero;
            
            StartCoroutine(StopDash());
        }

        private void Dash()
        {
            m_Rigidbody.AddForce(m_DashForce * Time.fixedDeltaTime * transform.right, ForceMode2D.Impulse);
        }

        private IEnumerator StopDash()
        {
            yield return new WaitForSeconds(m_DashTime);

            m_IsDashing = false;
            m_Rigidbody.gravityScale = m_GravityScale;
            m_Rigidbody.velocity = Vector2.zero;

            yield return new WaitForSeconds(m_NextDashTime);

            m_IsDashTime = true;
        }

        #endregion

        public void OnAttacking()
        {
            Vector2 velocity = m_Rigidbody.velocity;
            velocity.x = 0;
            m_Rigidbody.velocity = velocity;
        }

        public void OnPerformAttack(float moveY, float attackForce)
        {
            if (Mathf.Abs(moveY) < 0.5f)
                m_Rigidbody.AddForce(-transform.right * attackForce, ForceMode2D.Impulse);
            else m_Rigidbody.AddForce(attackForce * Mathf.Sign(moveY) * -transform.up, ForceMode2D.Impulse);
        }

        public void Animate(float move, float moveY)
        {
            if (!m_Animator)
                return;
            
            m_Animator.SetFloat(s_Speed, Mathf.Abs(move));
            m_Animator.SetFloat(s_YSpeed, moveY);
            m_Animator.SetFloat(s_VSpeed, m_Rigidbody.velocity.y);
            m_Animator.SetBool(s_IsGrounded, m_IsGrounded);
            m_Animator.SetBool(s_IsDashing, m_IsDashing);
            m_Animator.SetBool(s_IsTouchingWall, m_IsTouchingWall);
        }

        private void Flip()
        {
            transform.rotation = transform.eulerAngles.y == 0.0f ? Quaternion.Euler(0.0f, 180.0f, 0.0f) : Quaternion.Euler(0.0f, 0.0f, 0.0f);
            FacingRight = !FacingRight;
        }
    }
}
