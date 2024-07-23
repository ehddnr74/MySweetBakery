using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using static UnityEngine.GraphicsBuffer;

public class CustomerStackIdleState : StateMachineBehaviour
{
    CustomerController customerController;
    Counter counter;
    NavMeshAgent agent;
    Transform target;
    DisplayBread displayBread;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        customerController = animator.GetComponent<CustomerController>();
        displayBread = GameObject.Find("BreadDisplayArea").GetComponent<DisplayBread>();
        counter = GameObject.Find("Counter").GetComponent<Counter>();
        agent = animator.GetComponent<NavMeshAgent>();
    }


    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (customerController.customerState == CustomerState.ArriveToDisplayArea)
        {
            if (customerController.CustomerAblePickUp())
            {
                customerController.PickUpBread();
            }
        }

        if (customerController.customerState == CustomerState.ArriveToDisplayArea)
        {
            if (customerController.currentStackBread == customerController.maxStackBread)
            {
                if (counter.currentCounterIndex < counter.maxCounterIndex)
                {
                    displayBread.currentCustomerDisPlayAreaIndex--;
                    counter.currentCounterIndex++;
                    customerController.SetCounterPoint();
                    agent.SetDestination(customerController.entranceWayPoint.position);
                    target = customerController.entranceWayPoint;
                    customerController.customerState = CustomerState.MoveToEntrance;
                    animator.SetBool("ToStackWalk", true);
                    Sprite posSprite = SpriteDataBase.instance.GetSprite("계산");
                    customerController.WindowSetting(false, posSprite);
                    customerController.ReleaseDisplayWayPoint();
                }
            }
        }

        if (customerController.customerState == CustomerState.ArriveToCounter)
        {
            if (customerController.currentCounterLine == 1)
            {
                counter.counterCustomerQueue.Enqueue(animator.gameObject);
                customerController.customerState = CustomerState.Packaging;
            }

            if (customerController.currentCounterLine == 3)
            {
                if (customerController.CheckCounterSecondLineCount())
                {
                    customerController.ReleaseCounterWayPoint();
                    customerController.SetCounterPoint();
                    agent.SetDestination(customerController.counterWayPoint.position);
                    target = customerController.counterWayPoint;
                    customerController.currentCounterLine = 2;
                }
            }

            if (customerController.currentCounterLine == 2)
            {
                if(customerController.CheckCounterFirstLineCount())
                {
                    customerController.ReleaseCounterWayPoint();
                    customerController.SetCounterPoint();
                    agent.SetDestination(customerController.counterWayPoint.position);
                    target = customerController.counterWayPoint;
                    customerController.currentCounterLine = 1;
                }
            }  
        }


        if(customerController.customerState == CustomerState.Packaging)
        {
            if (counter.counterCustomerQueue.Count > 0
                && PlayerController.instance.CustomerAbleCalCulation())
            {
                GameObject customer = counter.counterCustomerQueue.Dequeue();
                CustomerController customerctrl = customer.GetComponent<CustomerController>();

                int customerBreadCount = customerctrl.currentStackBread;
                PaperBagManager.instance.GetObjectFromPool(customerctrl, customerBreadCount);
            }

            if (customerController.paperBag != null)
            {
                if (customerController.paperBag.finishContain)
                {
                    customerController.paperBag.finishContain = false;

                    agent.SetDestination(customerController.counterTurnToExitPoint.position);
                    target = customerController.counterTurnToExitPoint;

                    customerController.customerState = CustomerState.TurnToExit;
                    animator.SetBool("ToStackWalk", true);
                }
            }
        }




        if (target != null)
        {
            DirectRot(target);
        }
    }


    private void DirectRot(Transform _target) // 목표방향으로 즉시회전하기 위해 agent 자동회전제어 끄고 이 함수 사용
    {
        Vector3 dir = (_target.position - agent.transform.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        agent.transform.rotation = targetRot;
        target = null;
    }
}
