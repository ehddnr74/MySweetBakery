using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Counter counter;

    public Dictionary<string,Transform> arrowTr = new Dictionary<string,Transform>();

    private Coroutine arrowCoroutine;

    public GameObject arrowPrefab;
    private bool arrowChangeFlagDisPlay = false;
    private bool arrowChangeFlagCounter = false;

    public string currentArrowTarget;

    void Start()
    {
        counter = GameObject.Find("Counter").GetComponent<Counter>();

        Transform breadOvenArrowTr = GameObject.Find("BreadOvenArrowTr").GetComponent<Transform>();
        Transform breadDisplayAreaArrowTr = GameObject.Find("BreadDisplayAreaArrowTr").GetComponent<Transform>();
        Transform counterArrowTr = GameObject.Find("CounterArrowTr").GetComponent<Transform>();

        AddArrowTransform("오븐", breadOvenArrowTr);
        AddArrowTransform("진열대", breadDisplayAreaArrowTr);
        AddArrowTransform("카운터", counterArrowTr);

        SetArrowPosition(arrowTr["오븐"], "오븐"); // 초기 위치는 오븐의 ArrowTr로 설정 

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
            SetArrowPosition(arrowTr["진열대"], "진열대");
            arrowCoroutine = StartCoroutine(ArrowAnimation());
        }

        if(counter.counterCustomerQueue.Count > 0 && !arrowChangeFlagCounter)
        {
            arrowChangeFlagCounter = true;

            if(arrowCoroutine != null)
            {
                StopCoroutine(arrowCoroutine);
            }
            SetArrowPosition(arrowTr["카운터"], "카운터");
            arrowCoroutine = StartCoroutine(ArrowAnimation());
        }

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
