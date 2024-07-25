using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultIdleState : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        StartStateUpdate();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (HoldingBread())
        {
            animator.SetBool("ToStackIdle", true);
        }

        GetDirToState(animator);
    }

    private void GetDirToState(Animator animator)
    {
        Vector3 moveDirection = PlayerController.instance.GetMoveDirection();

        if (moveDirection != Vector3.zero)
        {
            animator.SetBool("ToDefaultMove", true);
        }
    }
    private void StartStateUpdate()
    {
        PlayerController.instance.playerState = PlayerState.DefaultIdle;
    }

    private bool HoldingBread()
    {
        return PlayerController.instance.breadStackCurrentCount > 0;
    }
}
