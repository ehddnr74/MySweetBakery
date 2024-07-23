using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackIdleState : StateMachineBehaviour
{
    private PlayerController playerController;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerController = animator.GetComponentInParent<PlayerController>();
        playerController.playerState = PlayerState.StackIdle;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector3 moveDirection = playerController.GetMoveDirection();
        if (moveDirection != Vector3.zero)
        {
            animator.SetBool("ToStackMove", true);
        }

        if (playerController.breadStackCurrentCount <= 0) 
        {
            animator.SetBool("ToStackIdle", false);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
