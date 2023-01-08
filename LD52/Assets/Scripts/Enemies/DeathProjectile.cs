using LD52.Player;
using UnityEngine;

namespace LD52.Enemies
{
    public class DeathProjectile : MonoBehaviour
    {
        [SerializeField] private float m_Speed = 200f;
        [SerializeField] private int m_IncreasedBlood = 10;

        private Rigidbody2D m_Rigidbody;

        private void Start()
        {
            m_Rigidbody = GetComponent<Rigidbody2D>();
            Destroy(gameObject, 5.0f);
        }

        private void FixedUpdate() => m_Rigidbody.velocity = transform.right * m_Speed * Time.fixedDeltaTime;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var player = collision.gameObject.GetComponent<PlayerStats>();
            if (!player)
                return;
                        
            Destroy(gameObject);
        }
    }
}