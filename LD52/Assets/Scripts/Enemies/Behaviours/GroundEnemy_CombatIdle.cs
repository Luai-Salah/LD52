using UnityEngine;

namespace LD52.Enemies.Behaviours
{
    public class GroundEnemy_CombatIdle : StateMachineBehaviour
    {
        private static readonly int s_Combat = Animator.StringToHash("Combat");

        [SerializeField] private float m_AttackDelay = 1f;
        
        private Transform m_Target;
        private Transform m_Transform;
        
        private float m_StopDistSqr;
        private float m_Timer;
        private static readonly int s_Attack = Animator.StringToHash("Attack");

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_Timer = m_AttackDelay;

            if (m_Target && m_Transform)
                return;
            
            var run = animator.GetBehaviour<GroundEnemy_Run>();
            
            m_Transform = run.EnemyAI.transform;
            m_Target = run.EnemyAI.Target;
            m_StopDistSqr = run.StopDistSqr;

            m_Timer = 0.1f;
        }
        
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (Utility.DistanceSqr(m_Transform.position, m_Target.position) > m_StopDistSqr)
                animator.SetBool(s_Combat, false);
            
            if (m_Timer <= 0)
                animator.SetTrigger(s_Attack);

            m_Timer -= Time.deltaTime;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.ResetTrigger(s_Attack);
        }
    }
}