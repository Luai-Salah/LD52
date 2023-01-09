using System.Collections;
using FMODUnity;
using UnityEngine;

namespace LD52.Enemies
{
    public class Enemy : MonoBehaviour
    {
        private static readonly int s_Hurt = Animator.StringToHash("Hurt");
        private static readonly int s_IsDead = Animator.StringToHash("IsDead");
        
        [SerializeField] private int m_MaxHealth = 100;
        [SerializeField] private StudioEventEmitter m_HitEmitter;

        private int m_CurHealth;
        private bool m_IsInvulnerable;

        private Animator m_Animator;
        private EnemyAI m_EnemyAI;

        private void Start()
        {
            m_CurHealth = m_MaxHealth;

            m_Animator = GetComponent<Animator>();
            m_EnemyAI = GetComponent<EnemyAI>();
        }

        public void TakeDamage(int damage, Vector2 direction) => StartCoroutine(TakeDamageImpl(damage, direction));

        private IEnumerator TakeDamageImpl(int damage, Vector2 direction)
        {
            if (m_IsInvulnerable)
                yield break;
            
            m_CurHealth -= damage;
            m_Animator.SetTrigger(s_Hurt);
            m_HitEmitter.Play();
            if (m_CurHealth <= 0)
                Die();

            m_IsInvulnerable = true;

            yield return new WaitForSeconds(.2f);

            m_IsInvulnerable = false;
        }

        private void Die()
        {
            m_Animator.SetBool(s_IsDead, true);
            m_EnemyAI.IsUpdating = false;
            m_EnemyAI.enabled = false;

            foreach (Collider2D col in GetComponents<Collider2D>())
                col.enabled = false;

            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        }
    }
}
