using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavArrow : MonoBehaviour
{
    private Arrow arrow;
    private Transform targetPosition; // Ÿ���� ���ο�
    public Transform playerTransform;
    public float distance; // �÷��̾�� ȭ��ǥ �Ÿ� 

    public Dictionary<string, Transform> navArrowTr = new Dictionary<string, Transform>();



    void Start()
    {
        arrow = GameObject.Find("Arrow").GetComponent<Arrow>();

        Transform breadOvenNavArrowTr = GameObject.Find("NavArrowOvenTr").GetComponent<Transform>();
        Transform breadDisplayAreaNavArrowTr = GameObject.Find("NavArrowDisplayTr").GetComponent<Transform>();
        Transform counterNavArrowTr = GameObject.Find("NavArrowCounterTr").GetComponent<Transform>();
        Transform counterMoneyNavArrowTr = GameObject.Find("CounterMoneyArrowTr").GetComponent<Transform>();
        Transform cafeteriaArrowTr = GameObject.Find("CafeteriaArrowTr").GetComponent<Transform>();

        AddNavArrowTransform("����", breadOvenNavArrowTr);
        AddNavArrowTransform("������", breadDisplayAreaNavArrowTr);
        AddNavArrowTransform("ī����", counterNavArrowTr);
        AddNavArrowTransform("ī���͵�", counterMoneyNavArrowTr);
        AddNavArrowTransform("ī��", cafeteriaArrowTr);


        //SetTargetPosition(arrow.currentArrowTarget);
    }
    void Update()
    {
        SetTargetPosition(arrow.currentArrowTarget);

        Vector3 directionToTarget = (targetPosition.position - playerTransform.position).normalized;
        directionToTarget.y = 0f;

        // ȭ��ǥ�� ��ġ�� ĳ���� ���� ������ �ִ� ��ó�� �ش��ϴ� ��ġ�� ��ġ
        Vector3 arrowPosition = playerTransform.position + directionToTarget * distance;

        transform.position = arrowPosition;

        Vector3 dir = targetPosition.position - transform.position;
        dir.y = 0f;

        Quaternion rot = Quaternion.LookRotation(dir.normalized);

        transform.rotation = rot;
    }

    private void AddNavArrowTransform(string name, Transform tr)
    {
        navArrowTr.Add(name, tr);
    }

    private void SetTargetPosition(string currentArrowTr)
    {
        targetPosition = navArrowTr[currentArrowTr];
    }
}
