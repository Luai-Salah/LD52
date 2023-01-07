using LD52.Player;
using UnityEngine;

namespace LD52.Enemies
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float m_Speed = 200f;
        [SerializeField] private int m_Damage = 10;

        private Rigidbody2D m_Rigidbody;

        private void Start()
        {
            m_Rigidbody = GetComponent<Rigidbody2D>();
            Destroy(gameObject, 5.0f);
        }

        private void FixedUpdate() => m_Rigidbody.velocity = transform.right * m_Speed * Time.fixedDeltaTime;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            collision.GetComponent<PlayerStats>()?.TakeDamage(m_Damage);
            Destroy(gameObject);
        }
    }
}
