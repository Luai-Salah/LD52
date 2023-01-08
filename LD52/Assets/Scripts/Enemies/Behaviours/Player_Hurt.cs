using UnityEngine;

namespace LD52.Enemies.Behaviours
{
    public class Player_Hurt : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            InputSystem.InputSystem.Player.Disable();
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            InputSystem.InputSystem.Player.Enable();
        }
    }
}