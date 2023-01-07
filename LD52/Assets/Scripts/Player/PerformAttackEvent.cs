using UnityEngine;

namespace LD52.Player
{
    public class PerformAttackEvent : MonoBehaviour
    {
        private PlayerCombat m_Combat;

        private void Start()
        {
            m_Combat = GetComponentInParent<PlayerCombat>();
        }

        public void PerformAttack() => m_Combat.PerformAttack();
    }
}
