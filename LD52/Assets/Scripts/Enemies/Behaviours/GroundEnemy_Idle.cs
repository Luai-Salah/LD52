using UnityEngine;

namespace LD52.Enemies.Behaviours
{
    public class GroundEnemy_Idle : StateMachineBehaviour
    {
        private static readonly int s_TargetInRange = Animator.StringToHash("TargetInRange");

        [SerializeField] private float m_TargetMaxDistance = 10f;

        public EnemyAI EnemyAI { get; private set; }
        public Transform Target { get; private set; }
        public Transform Transform { get; private set; }

        public float TargetMaxDistSqr { get; private set; }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (Target && Transform)
                return;

            EnemyAI = animator.GetComponent<EnemyAI>();
            
            Transform = EnemyAI.transform;
            Target = EnemyAI.Target;

            EnemyAI.IsUpdating = false;
            EnemyAI.FlipToTarget = false;

            TargetMaxDistSqr = m_TargetMaxDistance * m_TargetMaxDistance;

            EnemyAI.OnStart();
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            EnemyAI.IsUpdating = true;
            EnemyAI.FlipToTarget = true;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (Utility.DistanceSqr(Transform.position, Target.position) <= TargetMaxDistSqr)
                animator.SetBool(s_TargetInRange, true);
        }
    }
}