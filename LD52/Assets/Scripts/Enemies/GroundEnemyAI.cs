using UnityEngine;

namespace LD52.Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class GroundEnemyAI : EnemyAI
    {
        [Header("Ground Enemy")]
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

            float direction = Mathf.Sign(CurrentWaypoint.x - transform.position.x);
            Vector2 velocity = m_Rigidbody.velocity;
            velocity.x = m_Speed * direction * Time.fixedDeltaTime;

            m_Rigidbody.velocity = velocity;
            
            PostUpdate();
        }
    }
}