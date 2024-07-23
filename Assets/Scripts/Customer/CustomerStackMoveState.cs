using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using static UnityEngine.GraphicsBuffer;

public class CustomerStackMoveState : StateMachineBehaviour
{
    CustomerController customerController;
    Counter counter;
    NavMeshAgent agent;
    Transform target;
    DisplayBread displayBread;

    private bool ableSpawn;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        customerController = animator.GetComponent<CustomerController>();
        displayBread = GameObject.Find("BreadDisplayArea").GetComponent<DisplayBread>();
        counter = GameObject.Find("Counter").GetComponent<Counter>();
        agent = animator.GetComponent<NavMeshAgent>();
    }


    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(customerController.customerState == CustomerState.MoveToEntrance)
        {
            if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                customerController.customerState = CustomerState.MoveToCounter;
                agent.SetDestination(customerController.counterWayPoint.position);
                target = customerController.counterWayPoint;
                ableSpawn = true;
            }
        }

        if (customerController.customerState == CustomerState.MoveToCounter)
        {
            if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                customerController.customerState = CustomerState.ArriveToCounter;

                if (customerController.currentCounterLine == 1)
                {
                    counter.counterCustomerQueue.Enqueue(animator.gameObject);
                    customerController.customerState = CustomerState.Packaging;
                }
                animator.SetBool("ToStackWalk", false);
            }
        }

        if (customerController.customerState == CustomerState.TurnToExit)
        {
            if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                customerController.HideWindow();
                customerController.ShowEmoticon();
                customerController.ReleaseCounterWayPoint();
                agent.SetDestination(customerController.counterGoToExitPoint.position);
                target = customerController.counterGoToExitPoint;

                customerController.customerState = CustomerState.GoToExit;
            }
        }

        if (customerController.customerState == CustomerState.GoToExit)
        {
            if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                agent.SetDestination(customerController.turnToDoorPoint.position);
                target = customerController.turnToDoorPoint;

                customerController.customerState = CustomerState.TurnToDoor;
                counter.currentCounterIndex--;
            }
        }

        if (customerController.customerState == CustomerState.TurnToDoor)
        {
            if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                agent.SetDestination(customerController.goToDoorPoint.position);
                target = customerController.goToDoorPoint;

                customerController.customerState = CustomerState.GoToDoor;
            }
        }

        if (customerController.customerState == CustomerState.GoToDoor)
        {
            if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                PaperBagManager.instance.ReturnToPool(customerController.paperBag.gameObject);
                CustomerSpawner.instance.customerPool.ReturnObject(customerController.gameObject);
                customerController.paperBag.GetComponent<Animator>().Play("Paper Bag_appear", 0, 0f);
            }
        }



        if (target != null)
        {
            DirectRot(target);
        }

        if(ableSpawn)
        {
            ableSpawn = false;
            customerController.SpawnCustomer(1);
        }
    }


    private void DirectRot(Transform _target) // 목표방향으로 즉시회전하기 위해 agent 자동회전제어 끄고 이 함수 사용
    {
        Vector3 dir = (_target.position - agent.transform.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        agent.transform.rotation = targetRot;
        target = null;
    }

    private void RotationToObj(Transform objectTr) // 장소에 도착 후 오브젝트를 바라보는 함수 
    {
        Vector3 dir = (objectTr.position - agent.transform.position).normalized;
        dir.y = 0f;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        agent.transform.rotation = targetRot;
    }
}
