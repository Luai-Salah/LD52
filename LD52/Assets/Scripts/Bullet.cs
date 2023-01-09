using LD52.Enemies;
using UnityEngine;

namespace LD52
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float m_Speed = 20f;
        [SerializeField] private int m_Damage = 50;
        [SerializeField] private float m_TimeBeforeDestroy = 5f;

        private Rigidbody2D m_Rigidbody2D;

        private void Start()
        {
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            m_Rigidbody2D.velocity = transform.right * m_Speed;
            
            Destroy(gameObject, m_TimeBeforeDestroy);
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            col.GetComponent<Enemy>()?.TakeDamage(m_Damage);
        }
    }
}