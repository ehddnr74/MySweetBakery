using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultIdleState : StateMachineBehaviour
{
    private PlayerController playerController;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerController = animator.GetComponentInParent<PlayerController>();
        playerController.playerState = PlayerState.DefaultIdle;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector3 moveDirection = playerController.GetMoveDirection();
        if (moveDirection != Vector3.zero)
        {
            animator.SetBool("ToDefaultMove", true);
        }

        if(playerController.breadStackCurrentCount > 0)
        {
            animator.SetBool("ToStackIdle", true);
        }
    }
}
