using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerDefaultIdleState : StateMachineBehaviour
{
    CustomerController customerController;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        customerController = animator.GetComponent<CustomerController>();
    }


    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(customerController.customerState == CustomerState.ArriveToDisplayArea)
        {
            if(customerController.CustomerAblePickUp())
            {
                customerController.PickUpBread();
            }
        }

        if (customerController.currentStackBread > 0)
        {
            animator.SetBool("ToStackIdle", true);
        }
    }

}
