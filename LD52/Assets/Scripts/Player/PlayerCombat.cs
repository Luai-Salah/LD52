using System;
using System.Diagnostics.CodeAnalysis;
using FMODUnity;
using LD52.Enemies;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LD52.Player
{
    public class PlayerCombat : MonoBehaviour
    {
        public float MoveY { get; set; }

        public bool IsAttacking => m_NextAttackTime > Time.time;

        [SerializeField] private int m_DamageAmount = 20;
        [SerializeField] private float m_AttackCoolDown = 0.3f;
        
        [SerializeField] private LayerMask m_EnemiesLayer;
        [SerializeField] private Attack[] m_ComboAttacks;

        [Space]
        [SerializeField] private StudioEventEmitter m_SwordEmitter;

        private PlayerMotor m_Motor;
        private Animator m_Animator;

        private float m_NextAttackTime;
        private float m_AttackExitTime;
        private float m_CooldownTime;
        private int m_CurrentAttackIndex;
        private static readonly int s_Attack = Animator.StringToHash("Attack");
        private static readonly int s_VAttack = Animator.StringToHash("vAttack");
        private static readonly int s_AttackIndex = Animator.StringToHash("AttackIndex");

        private void Start()
        {
            m_Animator = GetComponentInChildren<Animator>();
            m_Motor = GetComponentInChildren<PlayerMotor>();
        }

        private void Update()
        {
            if (m_AttackExitTime > 0)
                m_AttackExitTime -= Time.deltaTime;
            else
            {
                m_AttackExitTime = 0;
                m_CurrentAttackIndex = 0;
            }
        }

        public void SetAttack(InputAction.CallbackContext _)
        {
            if (m_ComboAttacks.Length == 0)
                return;

            if (m_NextAttackTime >= Time.time || m_CooldownTime >= Time.time)
                return;
            
            Attack attack = m_ComboAttacks[m_CurrentAttackIndex];
            m_Animator.SetInteger(s_AttackIndex, m_CurrentAttackIndex);

            m_AttackExitTime = attack.AttackAnimation.length + 0.1f;
            m_NextAttackTime = Time.time + attack.AttackAnimation.length - 0.1f;

            Quaternion rotation = Quaternion.identity;
            Vector3 position = Vector3.zero;
            
            if (Mathf.Abs(MoveY) > 0.5f)
            {
                if (attack.VAttackPoint)
                {
                    rotation = MoveY > 0.01f ? Quaternion.Euler(0.0f, 0.0f, 90f) : Quaternion.Euler(0.0f, 0.0f, 270f);
                    position = attack.VAttackPoint.position;
                }
                
                m_Animator.SetTrigger(s_VAttack);
            }
            else
            {
                if (attack.AttackPoint)
                {
                    rotation = m_Motor.FacingRight ? Quaternion.identity : Quaternion.Euler(0.0f, 180f, 0.0f);
                    position = attack.AttackPoint.position;
                }
                
                m_Animator.SetTrigger(s_Attack);
            }

            if (attack.AttackPrefab)
                Destroy(Instantiate(attack.AttackPrefab, position, rotation), m_AttackExitTime);
            
            m_SwordEmitter.Play();
        }

        public void PerformAttack()
        {
            Attack attack = m_ComboAttacks[m_CurrentAttackIndex];

            if (attack.DamagePoint)
            {
                Vector3 pos, direction;
                if (Mathf.Abs(MoveY) > 0.5f)
                {
                    pos = attack.VDamagePoint.position;
                    direction = new Vector3(0.0f, Mathf.Sign(MoveY), 0.0f);
                }
                else
                {
                    pos = attack.DamagePoint.position;
                    direction = new Vector3(m_Motor.FacingRight ? 1.0f : -1.0f, 0.0f, 0.0f);
                }

                Collider2D[] enemies = Physics2D.OverlapCircleAll(pos, attack.AttackRadius, m_EnemiesLayer);

                foreach (Collider2D eCollider in enemies)
                {
                    eCollider.GetComponent<Enemy>()?.TakeDamage(m_DamageAmount);
                    var death = eCollider.GetComponent<DeathProjectile>();
                    if (death)
                        Destroy(death.gameObject);
                }
            }

            m_CurrentAttackIndex++;

            if (m_CurrentAttackIndex < m_ComboAttacks.Length)
                return;
            
            m_CurrentAttackIndex = 0;
            m_CooldownTime = Time.time + m_AttackCoolDown;
        }

        private void OnDrawGizmosSelected()
        {
            if (m_ComboAttacks == null)
                return;

            foreach (Attack t in m_ComboAttacks)
            {
                if (!t.DamagePoint)
                    continue;
                
                Gizmos.DrawWireSphere(t.DamagePoint.position,  t.AttackRadius);

                if (!t.VDamagePoint)
                    continue;
                
                Gizmos.DrawWireSphere(t.VDamagePoint.position, t.AttackRadius);
            }
        }
    }

    [Serializable]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Attack
    {
        public AnimationClip AttackAnimation;
        public GameObject AttackPrefab;
        public Transform AttackPoint;
        public Transform DamagePoint;
        public Transform VAttackPoint;
        public Transform VDamagePoint;
        public float AttackForce = 20f;
        public float AttackRadius = 0.3f;
    }
}
