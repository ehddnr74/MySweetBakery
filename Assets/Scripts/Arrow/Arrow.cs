using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Counter counter;
    private CounterStackMoney counterStackMoney;

    public GameObject arrowPrefab;

    private Coroutine arrowCoroutine;

    /////////// 화살표 위치 변화 플래그 
    private bool arrowChangeFlagDisPlay = false;
    private bool arrowChangeFlagCounter = false;
    private bool arrowChangeFlagCounterMoney = false;
    private bool arrowChangeFlagSealArea = false;
    private bool arrowChangeFlagSealArea2 = false;
    ///////////

    public string currentArrowTarget; // 현재 화살표의 타겟(NavArrow Update에서 Arrow의 Target으로 방향 전환함)

    public bool ableSealArea2Target;

    public Dictionary<string, Transform> arrowTr = new Dictionary<string, Transform>();

    void Start()
    {
        GetGameObject();
        AddArrowTransform(); // 화살표가 가지고 있어야 할 Transform들 여기서 추가  
        StartSetting(); // 가장 처음 위치, 코루틴 적용 여부 여기서 결정 
    }
    private void Update()
    {
        CheckAbleDisPlay(); 
        CheckAbleCounter();
        CheckAbleCounterMoney();
        CheckAbleSealArea1();
        CheckAbleSealArea2();
    }
    private IEnumerator ArrowAnimation()
    {
        float bounceHeight = 0.8f; // 위아래로 움직일 높이 여기서 정하면댐 !!
        Vector3 initialPosition = arrowPrefab.transform.position;

        while (true)
        {
            float elapsedTime = 0f;

            while (elapsedTime < 1.0f) // 내려갔다가 올라오는 왕복 시간이 1초임 !!
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Sin(elapsedTime * Mathf.PI);

                arrowPrefab.transform.position = new Vector3(initialPosition.x, initialPosition.y + t * bounceHeight, initialPosition.z);
                yield return null;
            }
        }
    }
    private void GetGameObject()
    {
        counter = GameObject.Find("Counter").GetComponent<Counter>();
        counterStackMoney = GameObject.Find("CounterMoneyManager").GetComponent<CounterStackMoney>();
    }
    private void SetArrowPosition(Transform tr, string _currentArrowTarget)
    {
        arrowPrefab.transform.SetParent(tr, true);
        arrowPrefab.transform.localPosition = Vector3.zero;
        currentArrowTarget = _currentArrowTarget;
    }
    private void AddArrowTransform()
    {
        Transform breadOvenArrowTr = GameObject.Find("BreadOvenArrowTr").GetComponent<Transform>();
        Transform breadDisplayAreaArrowTr = GameObject.Find("BreadDisplayAreaArrowTr").GetComponent<Transform>();
        Transform counterArrowTr = GameObject.Find("CounterArrowTr").GetComponent<Transform>();
        Transform counterMoneyArrowTr = GameObject.Find("CounterMoneyArrowTr").GetComponent<Transform>();
        Transform cafeteriaArrowTr = GameObject.Find("CafeteriaArrowTr").GetComponent<Transform>();
        Transform sealArea2ArrowTr = GameObject.Find("SealArea2ArrowTr").GetComponent<Transform>();

        arrowTr.Add("오븐", breadOvenArrowTr);
        arrowTr.Add("진열대", breadDisplayAreaArrowTr);
        arrowTr.Add("카운터", counterArrowTr);
        arrowTr.Add("카운터돈", counterMoneyArrowTr);
        arrowTr.Add("카페", cafeteriaArrowTr);
        arrowTr.Add("두번째봉인지역", sealArea2ArrowTr);
    }
    private void CheckAbleDisPlay()
    {
        if(arrowChangeFlagDisPlay)
        {
            return;
        }
        else
        {
            if (PlayerController.instance.breadStackCurrentCount > 0)
            {
                arrowChangeFlagDisPlay = true;
                ResetArrowTransform(arrowTr["진열대"], "진열대");
            }
        }
    }
    private void CheckAbleCounter()
    {
        if (arrowChangeFlagCounter)
        {
            return;
        }
        else
        {
            if (counter.counterCustomerQueue.Count > 0)
            {
                arrowChangeFlagCounter = true;
                ResetArrowTransform(arrowTr["카운터"], "카운터");
            }
        }
    }
    private void CheckAbleCounterMoney()
    {
        if (arrowChangeFlagCounterMoney)
        {
            return;
        }
        else
        {
            if (counterStackMoney.stackMoneyCount > 0)
            {
                arrowChangeFlagCounterMoney = true;
                ResetArrowTransform(arrowTr["카운터돈"], "카운터돈");
            }
        }
    }
    private void CheckAbleSealArea1()
    {
        if (arrowChangeFlagSealArea)
        {
            return;
        }
        else
        {
            if (PlayerController.instance.playerMoney >= 30)
            {
                arrowChangeFlagSealArea = true;
                ResetArrowTransform(arrowTr["카페"], "카페");
            }
        }
    }
    private void CheckAbleSealArea2()
    {
        if (arrowChangeFlagSealArea2)
        {
            return;
        }
        else
        {
            if (ableSealArea2Target)
            {
                arrowChangeFlagSealArea2 = true;
                ResetArrowTransform(arrowTr["두번째봉인지역"], "두번째봉인지역");
            }
        }
    }
    private void StartSetting()
    {
        SetArrowPosition(arrowTr["오븐"], "오븐"); // 초기 위치는 오븐의 ArrowTr로 설정 
        arrowCoroutine = StartCoroutine(ArrowAnimation());
    }
    private void ResetArrowTransform(Transform tr, string _currentArrowTarget)
    {
        if (arrowCoroutine != null)
        {
            StopCoroutine(arrowCoroutine);
        }
        SetArrowPosition(tr, _currentArrowTarget);
        arrowCoroutine = StartCoroutine(ArrowAnimation());
    }
}

