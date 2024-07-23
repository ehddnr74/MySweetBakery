using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DefaultMoveState : StateMachineBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    private PlayerController playerController;
    CharacterController characterController;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        characterController = animator.GetComponentInParent<CharacterController>();
        playerController = animator.GetComponentInParent<PlayerController>();
        playerController.playerState = PlayerState.DefaultMove;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector3 moveDirection = playerController.GetMoveDirection();

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            animator.transform.rotation = Quaternion.Slerp(animator.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
        }
        else
        {
            animator.SetBool("ToDefaultMove", false);
        }
    }
}
