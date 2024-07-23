using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class CustomerMoveState : StateMachineBehaviour
{
    CustomerController customerController;
    NavMeshAgent agent;
    Transform target;
    DisplayBread displayBread;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        displayBread = GameObject.Find("BreadDisplayArea").GetComponent<DisplayBread>();
        customerController = animator.GetComponent<CustomerController>();
        agent = animator.GetComponent<NavMeshAgent>();
        agent.speed = 4.5f;

        agent.updateRotation = false;
        agent.autoBraking = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (customerController.customerState == CustomerState.MoveToEntrance)
        {
            agent.SetDestination(customerController.entranceWayPoint.position);

            if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                customerController.customerState = CustomerState.MoveToDisplayArea;
                customerController.SetDisplayAreaPoint();
                agent.SetDestination(customerController.entranceWayPoint.position);
                target = customerController.breadDisplayWayPoint;
            }
        }

        if (customerController.customerState == CustomerState.MoveToDisplayArea)
        {
            if(displayBread.currentCustomerDisPlayAreaIndex == 1)
            {
                agent.SetDestination(customerController.breadDisplayWayPoint.position);
            }
            if (displayBread.currentCustomerDisPlayAreaIndex == 2)
            {
                agent.SetDestination(customerController.breadDisplayWayPoint.position);
            }
            if (displayBread.currentCustomerDisPlayAreaIndex == 3)
            {
                agent.SetDestination(customerController.breadDisplayWayPoint.position);
            }

            if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                customerController.customerState = CustomerState.ArriveToDisplayArea;
                animator.SetBool("ToIdle", true);

                Sprite breadSprite = SpriteDataBase.instance.GetSprite("빵");
                customerController.WindowSetting(true, breadSprite);
            }
        }

        if(customerController.customerState == CustomerState.ArriveToDisplayArea)
        {
            RotationToObj(customerController.objectTr["진열대"]);
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

    private void RotationToObj(Transform objectTr) // 장소에 도착 후 오브젝트를 바라보는 함수 
    {
        Vector3 dir = (objectTr.position - agent.transform.position).normalized;
        dir.y = 0f;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        agent.transform.rotation = targetRot;
    }
}

    
