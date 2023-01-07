using UnityEngine;

namespace LD52.Player
{
    [RequireComponent(typeof(PlayerController))]
    public class SoulsManager : MonoBehaviour
    {
        private PlayerController m_PlayerController;

        private MovementFlags m_BlueSoul;

        private void Awake()
        {
            m_PlayerController = GetComponent<PlayerController>();

            m_BlueSoul = MovementFlags.WallJump;
            m_PlayerController.MovementFlags = m_BlueSoul;
        }
    }
}