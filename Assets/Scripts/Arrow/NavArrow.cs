using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavArrow : MonoBehaviour
{
    private Arrow arrow;
    private Transform targetPosition; // Target = Arrow
    public Transform playerTransform;
    public float distance; // �÷��̾�� ȭ��ǥ �Ÿ� 

    public Dictionary<string, Transform> navArrowTr = new Dictionary<string, Transform>();

    void Start()
    {
        GetGameObject();
        AddNavArrowTransform(); // NavArrow�� �˾ƾ� �� Transform�� ���⼭ �߰�  
    }
    void Update()
    {
        SetTargetPosition(arrow.currentArrowTarget); // Arrow�� Transform�� Ÿ������ ���� 
        SetNavArrowTransformUpdate(); // NavArrow ������Ʈ
    }

    private void GetGameObject()
    {
        arrow = GameObject.Find("Arrow").GetComponent<Arrow>();
    }

    private void AddNavArrowTransform()
    {
        Transform breadOvenNavArrowTr = GameObject.Find("NavArrowOvenTr").GetComponent<Transform>();
        Transform breadDisplayAreaNavArrowTr = GameObject.Find("NavArrowDisplayTr").GetComponent<Transform>();
        Transform counterNavArrowTr = GameObject.Find("NavArrowCounterTr").GetComponent<Transform>();
        Transform counterMoneyNavArrowTr = GameObject.Find("CounterMoneyArrowTr").GetComponent<Transform>();
        Transform cafeteriaNavArrowTr = GameObject.Find("CafeteriaArrowTr").GetComponent<Transform>();
        Transform sealArea2NavArrowTr = GameObject.Find("SealArea2ArrowTr").GetComponent<Transform>();

        navArrowTr.Add("����", breadOvenNavArrowTr);
        navArrowTr.Add("������", breadDisplayAreaNavArrowTr);
        navArrowTr.Add("ī����", counterNavArrowTr);
        navArrowTr.Add("ī���͵�", counterMoneyNavArrowTr);
        navArrowTr.Add("ī��", cafeteriaNavArrowTr);
        navArrowTr.Add("�ι�°��������", sealArea2NavArrowTr);
    }

    private void SetTargetPosition(string currentArrowTr)
    {
        targetPosition = navArrowTr[currentArrowTr];
    }
    private void SetNavArrowTransformUpdate()
    {
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
}
