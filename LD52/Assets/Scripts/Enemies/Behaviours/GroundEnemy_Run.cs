using UnityEngine;

namespace LD52.Enemies.Behaviours
{
    public class GroundEnemy_Run : StateMachineBehaviour
    {
        private static readonly int s_Combat = Animator.StringToHash("Combat");

        public float StopDistSqr { get; private set; }
        public EnemyAI EnemyAI { get; private set; }

        [SerializeField] private float m_Speed = 200;
        [SerializeField] private float m_StopDistance = 2;
        
        private Transform m_Transform;
        private Transform m_Target;
        private Rigidbody2D m_Rigidbody2D;

        private float m_TargetMaxDistSqr;
        private static readonly int s_TargetInRange = Animator.StringToHash("TargetInRange");

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            StopDistSqr = m_StopDistance * m_StopDistance;
            
            if (m_Rigidbody2D) 
                return;

            var idle = animator.GetBehaviour<GroundEnemy_Idle>();
            
            EnemyAI = idle.EnemyAI;
            m_Transform = idle.Transform;
            m_Target = idle.Target;
            m_TargetMaxDistSqr = idle.TargetMaxDistSqr;
            
            m_Rigidbody2D = animator.GetComponent<Rigidbody2D>();
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Vector2 velocity = m_Rigidbody2D.velocity;
            velocity.x = 0;
            m_Rigidbody2D.velocity = velocity;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!EnemyAI.PreUpdate())
                return;
            
            float direction = Mathf.Sign(EnemyAI.CurrentWaypoint.x - m_Transform.position.x);
            Vector2 velocity = m_Rigidbody2D.velocity;
            velocity.x = m_Speed * direction * Time.fixedDeltaTime;

            m_Rigidbody2D.velocity = velocity;
            
            if (Utility.DistanceSqr(m_Transform.position, m_Target.position) <= StopDistSqr)
                animator.SetBool(s_Combat, true);
            
            if (Utility.DistanceSqr(m_Transform.position, m_Target.position) > m_TargetMaxDistSqr)
                animator.SetBool(s_TargetInRange, false);
            
            EnemyAI.PostUpdate();
        }
    }
}