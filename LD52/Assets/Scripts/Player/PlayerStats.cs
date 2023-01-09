using FMODUnity;
using UnityEngine;

namespace LD52.Player
{
    public class PlayerStats : MonoBehaviour
    {
        private static readonly int s_Hurt = Animator.StringToHash("Hurt");
        private static readonly int s_IsDead = Animator.StringToHash("IsDead");
        
        [Header("Health")]
        [SerializeField] private int m_MaxHealth = 100;

        [SerializeField] private StudioEventEmitter m_HitEmitter;
        [SerializeField] private StudioEventEmitter m_DeathEmitter;

        private Animator m_Animator;
        private Rigidbody2D m_Rigidbody2D;
        
        private bool m_Intense;
        
        private float m_CurrentHealth;
        private float m_CurrentBlood;

        private void Start()
        {
            m_Animator = GetComponentInChildren<Animator>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            
            m_CurrentHealth = m_MaxHealth;
        }

        public void TakeDamage(int amount)
        {
            m_CurrentHealth -= amount;
            m_Rigidbody2D.velocity = Vector2.zero;
            m_Animator.SetTrigger(s_Hurt);

            if (m_CurrentHealth <= 0)
                Die();
            else m_HitEmitter.Play();
        }

        private void Die()
        {
            foreach (Collider2D col in GetComponents<Collider2D>())
            {
                col.enabled = false;
            }

            GetComponent<PlayerController>().enabled = false;
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            
            m_Animator.SetBool(s_IsDead, true);
            m_DeathEmitter.Play();
        }
    }
}
