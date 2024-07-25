using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class CustomerDefaultMoveState : StateMachineBehaviour
{
    CustomerController customerController;
    NavMeshAgent agent;
    Transform target;
    DisplayBread displayBread;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GetGameObject(animator);
        SetAgent();
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        switch (customerController.customerState)
        {
            case CustomerState.MoveToEntrance:
                movetoentrance(animator);
                break;
            case CustomerState.MoveToDisplayArea:
                movetodisplayarea(animator);
                break;
            case CustomerState.ArriveToDisplayArea:
                arrivetodisplayarea(animator);
                break;
            case CustomerState.ExitToCafeteria:
                exittocafeteria(animator);
                break;
            case CustomerState.GoToDoorToCafeteria:
                gotodoortocafeteria(animator);
                break;
            default:
                break;
        }
        if (target != null) 
        {
            DirectRot(target);
        }
    }

    private void movetoentrance(Animator animator)
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

    private void movetodisplayarea(Animator animator)
    {
        if (displayBread.currentCustomerDisPlayAreaIndex == 1)
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

    private void arrivetodisplayarea(Animator animator)
    {
        RotationToObj(customerController.objectTr["진열대"]);
    }

    private void exittocafeteria(Animator animator)
    {
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            agent.SetDestination(customerController.goToDoorPoint.position);
            target = customerController.goToDoorPoint;
            customerController.customerState = CustomerState.GoToDoorToCafeteria;
        }
    }

    private void gotodoortocafeteria(Animator animator)
    {
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            CustomerSpawner.instance.customerPool.ReturnObject(customerController.gameObject);
        }
    }

    private void GetGameObject(Animator animator)
    {
        displayBread = GameObject.Find("BreadDisplayArea").GetComponent<DisplayBread>();
        customerController = animator.GetComponent<CustomerController>();
        agent = animator.GetComponent<NavMeshAgent>();
    }
    private void SetAgent()
    {
        agent.speed = 4.5f;
        agent.updateRotation = false;
        agent.autoBraking = false;
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

    
