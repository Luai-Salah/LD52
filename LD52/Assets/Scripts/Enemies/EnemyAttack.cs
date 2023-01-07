using System;
using UnityEngine;

namespace LD52.Enemies
{
    public class EnemyAttack : MonoBehaviour
    {
        [SerializeField] private GameObject m_Projectile;
        [SerializeField] private GameObject m_DeathProjectile;
        [SerializeField] private Transform m_Target;
        [SerializeField] private int m_Attacks = 4;
        [SerializeField] private float m_AttackRate = 4.0f;
        [SerializeField] private float m_AttackStartDelay = 0.5f;
        [SerializeField] private float m_Offset = 90f;

        private bool m_Attack;
        private int m_AttackIndex;

        private EnemyAI m_EnemyAI;
        private Animator m_Animator;
        private static readonly int s_Update = Animator.StringToHash("Update");

        private void Start()
        {
            m_EnemyAI = GetComponent<EnemyAI>();
            m_Animator = GetComponent<Animator>();

            if (!m_Target)
                m_Target = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Update()
        {
            m_Animator.SetBool(s_Update, m_EnemyAI.Update);
            
            switch (m_EnemyAI.ReachedEndOfPath)
            {
                case true when !m_Attack:
                    m_Attack = true;
                    m_EnemyAI.Update = false;
                    InvokeRepeating(nameof(Attack), m_AttackStartDelay, m_AttackRate);
                    break;
                case false:
                    m_Attack = false;
                    break;
            }

            if (m_AttackIndex < m_Attacks)
                return;
            
            CancelInvoke(nameof(Attack));
            m_EnemyAI.Update = true;
            m_AttackIndex = 0;
        }

        private void Attack()
        {
            Vector3 position = transform.position;
            
            Vector3 direction = (position - m_Target.position).normalized;
            float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            Instantiate(m_Projectile, position, Quaternion.Euler(new Vector3(0f, 0f, -angle - m_Offset)));
            m_AttackIndex++;
        }

        private void OnDestroy()
        {
            Vector3 position = transform.position;
            
            Vector3 direction = (position - m_Target.position).normalized;
            float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            Instantiate(m_DeathProjectile, position, Quaternion.Euler(new Vector3(0f, 0f, -angle - m_Offset)));
        }
    }
}
