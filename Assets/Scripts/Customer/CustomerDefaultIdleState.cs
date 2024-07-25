using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerDefaultIdleState : StateMachineBehaviour
{
    CustomerController customerController;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GetGameObject(animator);   
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        switch (customerController.customerState)
        {
            case CustomerState.ArriveToDisplayArea:
                arrivetodisplayarea(animator);
                break;
            default:  
                break;
        }
    }

    private void arrivetodisplayarea(Animator animator)
    {
        if (customerController.CustomerAblePickUp())
        {
            customerController.PickUpBread();
        }
        if (customerController.currentStackBread > 0)
        {
            animator.SetBool("ToStackIdle", true);
        }
    }
    private void GetGameObject(Animator animator)
    {
        customerController = animator.GetComponent<CustomerController>();
    }

}
