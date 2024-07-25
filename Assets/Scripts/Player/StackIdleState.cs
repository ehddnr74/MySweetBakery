using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackIdleState : StateMachineBehaviour
{

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        StartStateUpdate();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (NotHoldingBread())
        {
            animator.SetBool("ToStackIdle", false);
        }

        GetDirToState(animator);
    }

    private void GetDirToState(Animator animator)
    {
        Vector3 moveDirection = PlayerController.instance.GetMoveDirection();

        if (moveDirection != Vector3.zero)
        {
            animator.SetBool("ToStackMove", true);
        }
    }
    private void StartStateUpdate()
    {
        PlayerController.instance.playerState = PlayerState.StackIdle;
    }
    private bool NotHoldingBread()
    {
        return PlayerController.instance.breadStackCurrentCount <= 0;
    }
}
