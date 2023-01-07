using System;
using UnityEngine;

namespace LD52
{
    [RequireComponent(typeof(PlayerMotor))]
    [RequireComponent(typeof(PlayerCombat))]
    public class PlayerController : MonoBehaviour
    {
        private PlayerMotor m_Motor;
        private PlayerCombat m_Combat;
        private Weapon m_Weapon;

        private float m_Move;
        private float m_MoveY;

        private void Awake()
        {
            m_Motor = GetComponent<PlayerMotor>();
            m_Combat = GetComponent<PlayerCombat>();
            m_Weapon = GetComponent<Weapon>();
        }

        private void OnEnable()
        {
            InputSystem.InputSystem.Player.Enable();

            InputSystem.InputSystem.Player.Jump.performed += m_Motor.OnJump;
            InputSystem.InputSystem.Player.Jump.performed += m_Motor.OnWallJump;
            InputSystem.InputSystem.Player.Jump.canceled += m_Motor.OnJumpStop;

            InputSystem.InputSystem.Player.Dash.performed += m_Motor.OnDash;

            InputSystem.InputSystem.Player.Attack.performed += m_Combat.SetAttack;
            //InputSystem.InputSystem.Player.Shoot.performed += m_Weapon.Shoot;
        }

        private void OnDisable()
        {
            InputSystem.InputSystem.Player.Jump.performed -= m_Motor.OnJump;
            InputSystem.InputSystem.Player.Jump.performed -= m_Motor.OnWallJump;
            InputSystem.InputSystem.Player.Jump.canceled -= m_Motor.OnJumpStop;

            InputSystem.InputSystem.Player.Dash.performed -= m_Motor.OnDash;

            InputSystem.InputSystem.Player.Attack.performed -= m_Combat.SetAttack;
            //InputSystem.InputSystem.Player.Shoot.performed -= m_Weapon.Shoot;
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
    }
}
