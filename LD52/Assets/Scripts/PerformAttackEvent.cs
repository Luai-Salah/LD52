using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD52
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
