using FMODUnity;
using UnityEngine;

namespace LD52.Player
{
    public class PerformAttackEvent : MonoBehaviour
    {
        private PlayerCombat m_Combat;
        private StudioEventEmitter m_FootEmitter;

        private void Start()
        {
            m_Combat = GetComponentInParent<PlayerCombat>();
            m_FootEmitter = GetComponent<StudioEventEmitter>();
        }

        public void PerformAttack() => m_Combat.PerformAttack();
        private void FootStep() => m_FootEmitter.Play();
    }
}
