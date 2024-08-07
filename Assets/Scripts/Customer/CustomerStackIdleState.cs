using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class CustomerStackIdleState : StateMachineBehaviour
{
    CustomerController customerController;
    Counter counter;
    NavMeshAgent agent;
    Transform target;
    DisplayBread displayBread;
    CafeteriaStackMoney cafeteriaStackMoney;

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
            case CustomerState.ArriveToCounter:
                arrivetocounter(animator);
                break;
            case CustomerState.Packaging:
                packaging(animator);
                break;
            case CustomerState.ArriveToCounterForCafeteria:
                arrivetocounterforcateteria(animator);
                break;
            case CustomerState.ArriveToCafeteria:
                arrivetocafeteria(animator);
                break;
            default:
                break;
        }
        if (target != null)
        {
            DirectRot(target);
        }
    }

    private void arrivetodisplayarea(Animator animator)
    {
        if (customerController.CustomerAblePickUp())
        {
            customerController.PickUpBread();
        }

        if (customerController.spawnCount != 4)
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
        else
        {
            if (customerController.currentStackBread == customerController.maxStackBread)
            {
                displayBread.currentCustomerDisPlayAreaIndex--;
                agent.SetDestination(customerController.entranceWayPoint.position);
                target = customerController.entranceWayPoint;
                customerController.customerState = CustomerState.MoveToEntrance;
                animator.SetBool("ToStackWalk", true);
                customerController.ReleaseDisplayWayPoint();
            }
        }
    }
    private void arrivetocounter(Animator animator)
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
            if (customerController.CheckCounterFirstLineCount())
            {
                customerController.ReleaseCounterWayPoint();
                customerController.SetCounterPoint();
                agent.SetDestination(customerController.counterWayPoint.position);
                target = customerController.counterWayPoint;
                customerController.currentCounterLine = 1;
            }
        }
    }
    private void packaging(Animator animator)
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
    private void arrivetocounterforcateteria(Animator animator)
    {
        if (PlayerController.instance.cafeteria != null)
        {
            if (PlayerController.instance.cafeteria.customerAbleCafeteria)
            {
                agent.SetDestination(customerController.goToCafeteriaPoint.position);
                target = customerController.goToCafeteriaPoint;

                customerController.HideWindow();

                customerController.customerState = CustomerState.GoToCafeteria;
                animator.SetBool("ToStackWalk", true);
            }
        }
    }
    private void arrivetocafeteria(Animator animator)
    {
        if (customerController.enjoyTimeOverCafetria)
        {
            agent.SetDestination(customerController.turnToDoorPoint.position);
            target = customerController.turnToDoorPoint;
            customerController.ShowEmoticon();

            cafeteriaStackMoney.SpawnMoney(10);
            SoundPool.instance.PlaySound("StackMoney");

            if (PlayerController.instance.cafeteria.cafeteriaChair != null)
            {
                GameObject obj = PlayerController.instance.cafeteria.cafeteriaChair;
                obj.transform.rotation *= Quaternion.Euler(0f, 45f, 0f);
            }

            animator.SetBool("ToIdle", false);
            animator.SetBool("ToWalk", true);
            customerController.customerState = CustomerState.ExitToCafeteria;
        }
    }
    private void GetGameObject(Animator animator)
    {
        customerController = animator.GetComponent<CustomerController>();
        displayBread = GameObject.Find("BreadDisplayArea").GetComponent<DisplayBread>();
        counter = GameObject.Find("Counter").GetComponent<Counter>();
        cafeteriaStackMoney = GameObject.Find("CafeTeriaMoneyManager").GetComponent<CafeteriaStackMoney>();
        agent = animator.GetComponent<NavMeshAgent>();
    }
    private void DirectRot(Transform _target) // 목표방향으로 즉시회전하기 위해 agent 자동회전제어 끄고 이 함수 사용
    {
        Vector3 dir = (_target.position - agent.transform.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(dir);
        agent.transform.rotation = targetRot;
        target = null;
    }
}
