using UnityEngine;
using UnityEngine.InputSystem;

namespace LD52.Player
{
    [RequireComponent(typeof(PlayerMotor))]
    [RequireComponent(typeof(PlayerCombat))]
    public class PlayerController : MonoBehaviour
    {
        public MovementFlags MovementFlags { get; set; }

        private PlayerMotor m_Motor;
        private PlayerCombat m_Combat;

        private float m_Move;
        private float m_MoveY;

        private void Awake()
        {
            m_Motor = GetComponent<PlayerMotor>();
            m_Combat = GetComponent<PlayerCombat>();
            
            InputSystem.InputSystem.Player.Jump.performed += m_Motor.OnJump;
            InputSystem.InputSystem.Player.Jump.canceled += m_Motor.OnJumpStop;
            
            InputSystem.InputSystem.Player.Jump.performed += WallJump;

            InputSystem.InputSystem.Player.Dash.performed += Dash;

            InputSystem.InputSystem.Player.Attack.performed += m_Combat.SetAttack;
            //InputSystem.InputSystem.Player.Shoot.performed += m_Weapon.Shoot;
        }

        private void OnEnable()
        {
            InputSystem.InputSystem.Player.Enable();
        }

        private void OnDisable()
        {
            InputSystem.InputSystem.Player.Disable();
        }

        private void Update()
        {
            m_Move = InputSystem.InputSystem.Player.Move.ReadValue<float>();
            m_MoveY = InputSystem.InputSystem.Player.MoveY.ReadValue<float>();
            
            m_Motor.Animate(m_Move, m_MoveY);
        }

        private void FixedUpdate()
        {
            if (m_Combat.IsAttacking)
                m_Motor.OnAttacking();
            
            m_Motor.Move(m_Move);
            m_Combat.MoveY = m_MoveY;
        }

        private void Dash(InputAction.CallbackContext _)
        {
            if (MovementFlags.HasFlag(MovementFlags.Dash))
                m_Motor.OnDash();
        }

        private void WallJump(InputAction.CallbackContext _)
        {
            if (MovementFlags.HasFlag(MovementFlags.WallJump))
                m_Motor.OnWallJump();
        }
    }
}
