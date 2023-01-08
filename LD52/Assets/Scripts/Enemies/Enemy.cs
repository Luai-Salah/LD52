using System.Collections;
using UnityEngine;

namespace LD52.Enemies
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private int m_MaxHealth = 100;

        private int m_CurHealth;

        private Rigidbody2D m_RigidBody;
        private SpriteRenderer m_SpriteRenderer;

        private void Start()
        {
            m_CurHealth = m_MaxHealth;

            m_RigidBody = GetComponentInChildren<Rigidbody2D>();
            m_SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        public void TakeDamage(int damage, Vector2 direction) => StartCoroutine(TakeDamageImpl(damage, direction));

        private IEnumerator TakeDamageImpl(int damage, Vector2 direction)
        {
            m_CurHealth -= damage;
            if (m_CurHealth <= 0)
            {
                Destroy(gameObject);
            }

            m_RigidBody.AddForce(direction * 10.0f, ForceMode2D.Impulse);

            for (short i = 0; i < 3; i++)
            {
                m_SpriteRenderer.color = Color.red;

                yield return new WaitForSeconds(0.1f);

                // ReSharper disable once Unity.InefficientPropertyAccess
                m_SpriteRenderer.color = Color.white;
            }
        }
    }
}
