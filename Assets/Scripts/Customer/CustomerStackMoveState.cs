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
    Money money;

    private bool ableSpawn;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        customerController = animator.GetComponent<CustomerController>();
        counter = GameObject.Find("Counter").GetComponent<Counter>();
        money = GameObject.Find("MoneyManager").GetComponent<Money>();
        agent = animator.GetComponent<NavMeshAgent>();
    }


    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(customerController.customerState == CustomerState.MoveToEntrance)
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

        if (customerController.customerState == CustomerState.MoveToCounterForCafeteria)
        {
            if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                customerController.customerState = CustomerState.ArriveToCounterForCafeteria;

                RotationToObj(customerController.objectTr["ī����"]);
                Sprite chairSprite = SpriteDataBase.instance.GetSprite("����");
                customerController.WindowSetting(false, chairSprite);
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

                if (customerController.paperBag.moneyCount == 1)
                {
                    money.SpawnMoney(5);
                }
                if (customerController.paperBag.moneyCount == 2)
                {
                    money.SpawnMoney(10);
                }
                if (customerController.paperBag.moneyCount == 3)
                {
                    money.SpawnMoney(20);
                }

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
            }
        }



        if (target != null)
        {
            DirectRot(target);
        }

        if(ableSpawn)
        {
            ableSpawn = false;
            customerController.AutoSpawnCustomer();
        }
    }


    private void DirectRot(Transform _target) // ��ǥ�������� ���ȸ���ϱ� ���� agent �ڵ�ȸ������ ���� �� �Լ� ���
    {
        Vector3 dir = (_target.position - agent.transform.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        agent.transform.rotation = targetRot;
        target = null;
    }

    private void RotationToObj(Transform objectTr) // ��ҿ� ���� �� ������Ʈ�� �ٶ󺸴� �Լ� 
    {
        Vector3 dir = (objectTr.position - agent.transform.position).normalized;
        dir.y = 0f;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        agent.transform.rotation = targetRot;
    }
}
