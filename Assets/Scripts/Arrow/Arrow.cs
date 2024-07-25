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

    /////////// ȭ��ǥ ��ġ ��ȭ �÷��� 
    private bool arrowChangeFlagDisPlay = false;
    private bool arrowChangeFlagCounter = false;
    private bool arrowChangeFlagCounterMoney = false;
    private bool arrowChangeFlagSealArea = false;
    private bool arrowChangeFlagSealArea2 = false;
    ///////////

    public string currentArrowTarget; // ���� ȭ��ǥ�� Ÿ��(NavArrow Update���� Arrow�� Target���� ���� ��ȯ��)

    public bool ableSealArea2Target;

    public Dictionary<string, Transform> arrowTr = new Dictionary<string, Transform>();

    void Start()
    {
        GetGameObject();
        AddArrowTransform(); // ȭ��ǥ�� ������ �־�� �� Transform�� ���⼭ �߰�  
        StartSetting(); // ���� ó�� ��ġ, �ڷ�ƾ ���� ���� ���⼭ ���� 
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
        float bounceHeight = 0.8f; // ���Ʒ��� ������ ���� ���⼭ ���ϸ�� !!
        Vector3 initialPosition = arrowPrefab.transform.position;

        while (true)
        {
            float elapsedTime = 0f;

            while (elapsedTime < 1.0f) // �������ٰ� �ö���� �պ� �ð��� 1���� !!
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

        arrowTr.Add("����", breadOvenArrowTr);
        arrowTr.Add("������", breadDisplayAreaArrowTr);
        arrowTr.Add("ī����", counterArrowTr);
        arrowTr.Add("ī���͵�", counterMoneyArrowTr);
        arrowTr.Add("ī��", cafeteriaArrowTr);
        arrowTr.Add("�ι�°��������", sealArea2ArrowTr);
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
                ResetArrowTransform(arrowTr["������"], "������");
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
                ResetArrowTransform(arrowTr["ī����"], "ī����");
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
                ResetArrowTransform(arrowTr["ī���͵�"], "ī���͵�");
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
                ResetArrowTransform(arrowTr["ī��"], "ī��");
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
                ResetArrowTransform(arrowTr["�ι�°��������"], "�ι�°��������");
            }
        }
    }
    private void StartSetting()
    {
        SetArrowPosition(arrowTr["����"], "����"); // �ʱ� ��ġ�� ������ ArrowTr�� ���� 
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

