using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LD52
{
    public class PlayerStats : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private int m_MaxHealth = 100;

        [Space]
        [SerializeField] private Slider m_HealthSlider;
        [SerializeField] private RectTransform m_HealthRect;

        [Header("Blood")]
        [SerializeField] private int m_MaxBlood = 100;
        [SerializeField] private float m_BloodConsumptionMultiplier = 2f;
        
        [Space]
        [SerializeField] private Slider m_BloodSlider;
        [SerializeField] private RectTransform m_BloodRect;

        private bool m_Intense;
        
        private float m_CurrentHealth;
        private float m_CurrentBlood;

        private void Start()
        {
            m_CurrentHealth = m_MaxHealth;

            m_HealthSlider.maxValue = m_MaxHealth;
            m_HealthRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_MaxHealth);

            m_BloodSlider.maxValue = m_MaxBlood;
            m_BloodRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_MaxBlood);
        }

        private void Update()
        {
            ConsumeBlood(Time.deltaTime * m_BloodConsumptionMultiplier);
            
            m_HealthSlider.value = m_CurrentHealth;
            m_BloodSlider.value = m_CurrentBlood;

            if (m_CurrentHealth > m_MaxHealth / 4f && m_CurrentBlood < m_MaxBlood - m_MaxBlood / 4f && m_Intense)
            {
                m_Intense = false;
            }
        }

        public bool ConsumeBlood(float amount)
        {
            if (m_CurrentBlood < amount)
                return false;
            
            m_CurrentBlood -= amount;
            
            if ((m_CurrentHealth < m_MaxHealth / 4f || m_CurrentBlood >= m_MaxBlood - m_MaxBlood / 4f) && !m_Intense)
            {
                m_Intense = true;
            }

            return true;
        }

        public void TakeDamage(int amount)
        {
            m_CurrentHealth -= amount;
            if (m_CurrentHealth <= 0)
                Die();
        }

        private void Die()
        {
            foreach (Collider2D col in GetComponents<Collider2D>())
            {
                col.enabled = false;
            }

            GetComponent<PlayerController>().enabled = false;
            GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
