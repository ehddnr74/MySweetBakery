using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultIdleState : StateMachineBehaviour
{

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerController.instance.playerState = PlayerState.DefaultIdle;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector3 moveDirection = PlayerController.instance.GetMoveDirection();
        if (moveDirection != Vector3.zero)
        {
            animator.SetBool("ToDefaultMove", true);
        }

        if(PlayerController.instance.breadStackCurrentCount > 0)
        {
            animator.SetBool("ToStackIdle", true);
        }
    }
}
