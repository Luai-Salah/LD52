using UnityEngine;

namespace LD52.Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class FlyingEnemyAI : EnemyAI
    {
        [Header("Flying Enemy")]
        [SerializeField] private float m_Speed = 200f;
        
        private Rigidbody2D m_Rigidbody;

        private void Start()
        {
            OnStart();
            
            m_Rigidbody = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            if (!PreUpdate())
                return;

            Vector2 direction = (CurrentWaypoint - transform.position).normalized;
            Vector2 force = m_Speed * Time.fixedDeltaTime * direction;

            m_Rigidbody.AddForce(force);
            
            PostUpdate();
        }
    }
}