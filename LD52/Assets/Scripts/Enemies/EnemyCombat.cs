using System;
using LD52.Player;
using UnityEngine;

namespace LD52.Enemies
{
    public class EnemyCombat : MonoBehaviour
    {
        [SerializeField] private int m_Damage = 10;
        [SerializeField] private Transform m_AttackPoint;
        [SerializeField] private LayerMask m_PlayerLayer;
        [SerializeField] private float m_AttackRadius;

        private void PerformAttack()
        {
            Collider2D col = Physics2D.OverlapCircle(m_AttackPoint.position, m_AttackRadius, m_PlayerLayer);
            PlayerStats player = col ? col.GetComponent<PlayerStats>() : null;

            if (player)
            {
                player.TakeDamage(m_Damage);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (m_AttackPoint)
                Gizmos.DrawWireSphere(m_AttackPoint.position, m_AttackRadius);
        }
    }
}