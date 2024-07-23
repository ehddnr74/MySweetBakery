using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Counter counter;
    private Money money;

    public Dictionary<string,Transform> arrowTr = new Dictionary<string,Transform>();

    private Coroutine arrowCoroutine;

    public GameObject arrowPrefab;
    private bool arrowChangeFlagDisPlay = false;
    private bool arrowChangeFlagCounter = false;
    private bool arrowChangeFlagCounterMoney = false;
    private bool arrowChangeFlagSealArea = false;

    public string currentArrowTarget;

    void Start()
    {
        counter = GameObject.Find("Counter").GetComponent<Counter>();
        money = GameObject.Find("MoneyManager").GetComponent<Money>();

        Transform breadOvenArrowTr = GameObject.Find("BreadOvenArrowTr").GetComponent<Transform>();
        Transform breadDisplayAreaArrowTr = GameObject.Find("BreadDisplayAreaArrowTr").GetComponent<Transform>();
        Transform counterArrowTr = GameObject.Find("CounterArrowTr").GetComponent<Transform>();
        Transform counterMoneyArrowTr = GameObject.Find("CounterMoneyArrowTr").GetComponent<Transform>();
        Transform cafeteriaArrowTr = GameObject.Find("CafeteriaArrowTr").GetComponent<Transform>();

        AddArrowTransform("����", breadOvenArrowTr);
        AddArrowTransform("������", breadDisplayAreaArrowTr);
        AddArrowTransform("ī����", counterArrowTr);
        AddArrowTransform("ī���͵�", counterMoneyArrowTr);
        AddArrowTransform("ī��", cafeteriaArrowTr);

        SetArrowPosition(arrowTr["����"], "����"); // �ʱ� ��ġ�� ������ ArrowTr�� ���� 

        if (gameObject.activeInHierarchy)
        {
            arrowCoroutine = StartCoroutine(ArrowAnimation());
        }
    }

    private void Update()
    {
        if(PlayerController.instance.breadStackCurrentCount > 0 && !arrowChangeFlagDisPlay)
        {
            arrowChangeFlagDisPlay = true;

            if (arrowCoroutine != null)
            {
                StopCoroutine(arrowCoroutine);
            }
            SetArrowPosition(arrowTr["������"], "������");
            arrowCoroutine = StartCoroutine(ArrowAnimation());
        }

        if(counter.counterCustomerQueue.Count > 0 && !arrowChangeFlagCounter)
        {
            arrowChangeFlagCounter = true;

            if(arrowCoroutine != null)
            {
                StopCoroutine(arrowCoroutine);
            }
            SetArrowPosition(arrowTr["ī����"], "ī����");
            arrowCoroutine = StartCoroutine(ArrowAnimation());
        }

        if(money.stackMoneyCount > 0 && !arrowChangeFlagCounterMoney)
        {
            arrowChangeFlagCounterMoney = true;

            if (arrowCoroutine != null)
            {
                StopCoroutine(arrowCoroutine);
            }

            SetArrowPosition(arrowTr["ī���͵�"], "ī���͵�");
            arrowCoroutine = StartCoroutine(ArrowAnimation());
        }

        if(PlayerController.instance.playerMoney >= 30 && !arrowChangeFlagSealArea)
        {
            arrowChangeFlagSealArea = true;

            if (arrowCoroutine != null)
            {
                StopCoroutine(arrowCoroutine);
            }

            SetArrowPosition(arrowTr["ī��"], "ī��");
            arrowCoroutine = StartCoroutine(ArrowAnimation());
        }
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

    private void AddArrowTransform(string name, Transform tr)
    {
        arrowTr.Add(name, tr);
    }

    private void SetArrowPosition(Transform tr, string _currentArrowTarget)
    {
        arrowPrefab.transform.SetParent(tr, true);
        arrowPrefab.transform.localPosition = Vector3.zero;
        currentArrowTarget = _currentArrowTarget;
    }
}
