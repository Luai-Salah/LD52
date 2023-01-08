using LD52.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LD52
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private Transform m_FirePoint;
        [SerializeField] private GameObject m_BulletPrefab;

        [SerializeField] private float m_BloodCost = 20f;

        private PlayerStats m_PlayerStats;
        private Animator m_Animator;
        private static readonly int s_Shoot = Animator.StringToHash("Shoot");

        private void Start()
        {
            m_PlayerStats = GetComponent<PlayerStats>();
            m_Animator = GetComponentInChildren<Animator>();
        }

        public void Shoot(InputAction.CallbackContext _)
        {
            Instantiate(m_BulletPrefab, m_FirePoint.position, m_FirePoint.rotation);
            m_Animator.SetTrigger(s_Shoot);
        }
    }
}
