using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private PlayerController playerController;

    public Dictionary<string,Transform> arrowTr = new Dictionary<string,Transform>();

    private Coroutine arrowCoroutine;

    private bool arrowChangeFlag = false;

    void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();

        Transform breadOvenArrowTr = GameObject.Find("BreadOvenArrowTr").GetComponent<Transform>();
        Transform breadDisplayAreaTr = GameObject.Find("BreadDisplayAreaArrowTr").GetComponent<Transform>();

        AddArrowTransform("����", breadOvenArrowTr);
        AddArrowTransform("������", breadDisplayAreaTr);

        SetArrowPosition(arrowTr["����"]); // �ʱ� ��ġ�� ������ ArrowTr�� ���� 

        if (gameObject.activeInHierarchy)
        {
            arrowCoroutine = StartCoroutine(ArrowAnimation());
        }
    }

    private void Update()
    {
        if(playerController.breadStackCurrentCount > 0 && !arrowChangeFlag)
        {
            arrowChangeFlag = true;

            if (arrowCoroutine != null)
            {
                StopCoroutine(arrowCoroutine);
            }
            SetArrowPosition(arrowTr["������"]);
            arrowCoroutine = StartCoroutine(ArrowAnimation());
        }
    }

    private IEnumerator ArrowAnimation()
    {
        float bounceHeight = 0.8f; // ���Ʒ��� ������ ���� ���⼭ ���ϸ�� !!
        Vector3 initialPosition = transform.position;

        while (true)
        {
            float elapsedTime = 0f;

            while (elapsedTime < 1.0f) // �������ٰ� �ö���� �պ� �ð��� 1���� !!
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Sin(elapsedTime * Mathf.PI);

                transform.position = new Vector3(initialPosition.x, initialPosition.y + t * bounceHeight, initialPosition.z);
                yield return null;
            }
        }
    }

    private void AddArrowTransform(string name, Transform tr)
    {
        arrowTr.Add(name, tr);
    }

    private void SetArrowPosition(Transform tr)
    {
        transform.SetParent(tr, true);
        transform.localPosition = Vector3.zero;
    }
}
