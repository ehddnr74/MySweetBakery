using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class CustomerStackMoveState : StateMachineBehaviour
{
    CustomerController customerController;
    Counter counter;
    NavMeshAgent agent;
    Transform target;
    CounterStackMoney counterStackMoney;

    private bool ableSpawn;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GetGameObject(animator);
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        switch (customerController.customerState)
        {
            case CustomerState.MoveToEntrance:
                movetoentrance(animator);
                break;
            case CustomerState.MoveToCounter:
                movetocounter(animator);
                break;
            case CustomerState.MoveToCounterForCafeteria:
                movetocounterforcafeteria(animator);
                break;
            case CustomerState.TurnToExit:
                turntoexit(animator);
                break;
            case CustomerState.GoToExit:
                gotoexit(animator);
                break;
            case CustomerState.TurnToDoor:
                turntodoor(animator);
                break;
            case CustomerState.GoToDoor:
                gotodoor(animator);
                break;
            case CustomerState.GoToCafeteria:
                gotocafeteria(animator);
                break;
            default:
                break;
        }

        if (target != null)
        {
            DirectRot(target);
        }

        if (ableSpawn)
        {
            ableSpawn = false;
            customerController.AutoSpawnCustomer();
        }
    }

    private void movetoentrance(Animator animator)
    {
        if (customerController.spawnCount != 4)
        {
            if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                customerController.customerState = CustomerState.MoveToCounter;
                agent.SetDestination(customerController.counterWayPoint.position);
                target = customerController.counterWayPoint;
                ableSpawn = true;
            }
        }
        else
        {
            customerController.customerState = CustomerState.MoveToCounterForCafeteria;
            agent.SetDestination(customerController.counterForCafeteriaWayPoint.position);
            target = customerController.counterForCafeteriaWayPoint;
            ableSpawn = true;
        }
    }
    private void movetocounter(Animator animator)
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
    private void movetocounterforcafeteria(Animator animator)
    {
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            customerController.customerState = CustomerState.ArriveToCounterForCafeteria;

            RotationToObj(customerController.objectTr["카운터"]);
            Sprite chairSprite = SpriteDataBase.instance.GetSprite("의자");
            customerController.WindowSetting(false, chairSprite);
            animator.SetBool("ToStackWalk", false);
        }
    }
    private void turntoexit(Animator animator)
    {
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            customerController.HideWindow();
            customerController.ShowEmoticon();
            customerController.ReleaseCounterWayPoint();
            agent.SetDestination(customerController.counterGoToExitPoint.position);
            target = customerController.counterGoToExitPoint;

            if (customerController.paperBag.moneyCount == 1)
            {
                counterStackMoney.SpawnMoney(5);
                SoundPool.instance.PlaySound("StackMoney");
            }
            if (customerController.paperBag.moneyCount == 2)
            {
                counterStackMoney.SpawnMoney(10);
                SoundPool.instance.PlaySound("StackMoney");
            }
            if (customerController.paperBag.moneyCount == 3)
            {
                counterStackMoney.SpawnMoney(20);
                SoundPool.instance.PlaySound("StackMoney");
            }

            customerController.customerState = CustomerState.GoToExit;
        }
    }
    private void gotoexit(Animator animator)
    {
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            agent.SetDestination(customerController.turnToDoorPoint.position);
            target = customerController.turnToDoorPoint;

            customerController.customerState = CustomerState.TurnToDoor;
            counter.currentCounterIndex--;
        }
    }
    private void turntodoor(Animator animator)
    {
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            agent.SetDestination(customerController.goToDoorPoint.position);
            target = customerController.goToDoorPoint;

            customerController.customerState = CustomerState.GoToDoor;
        }
    }
    private void gotodoor(Animator animator)
    {
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            PaperBagManager.instance.ReturnToPool(customerController.paperBag.gameObject);
            CustomerSpawner.instance.customerPool.ReturnObject(customerController.gameObject);
        }
    }
    private void gotocafeteria(Animator animator)
    {
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            animator.SetBool("ToStackWalk", false);
            RotationToObj(customerController.objectTr["카페"]);
            if (PlayerController.instance.cafeteria.BreadTr != null)
            {
                customerController.CafeteriaSettingBread(PlayerController.instance.cafeteria.BreadTr.transform);
            }
            customerController.customerState = CustomerState.ArriveToCafeteria;
        }
    }
    private void GetGameObject(Animator animator)
    {
        customerController = animator.GetComponent<CustomerController>();
        counter = GameObject.Find("Counter").GetComponent<Counter>();
        counterStackMoney = GameObject.Find("CounterMoneyManager").GetComponent<CounterStackMoney>();
        agent = animator.GetComponent<NavMeshAgent>();
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
