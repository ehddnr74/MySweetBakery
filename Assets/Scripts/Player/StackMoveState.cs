using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackMoveState : StateMachineBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    CharacterController characterController;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GetGameObject(animator);
        StartStateUpdate();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GetDirToState(animator);          
    }

    private void GetDirToState(Animator animator)
    {
        Vector3 moveDirection = PlayerController.instance.GetMoveDirection();

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            animator.transform.rotation = Quaternion.Slerp(animator.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
        }
        else
        {
            animator.SetBool("ToStackMove", false);
        }
    }
    private void StartStateUpdate()
    {
        PlayerController.instance.playerState = PlayerState.StackMove;
    }
    private void GetGameObject(Animator animator)
    {
        characterController = animator.GetComponentInParent<CharacterController>();
    }
}
