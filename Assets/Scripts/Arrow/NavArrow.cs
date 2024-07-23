using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavArrow : MonoBehaviour
{
    private Arrow arrow;
    private Transform targetPosition; // 타겟은 에로우
    public Transform playerTransform;
    public float distance; // 플레이어와 화살표 거리 

    public Dictionary<string, Transform> navArrowTr = new Dictionary<string, Transform>();



    void Start()
    {
        arrow = GameObject.Find("Arrow").GetComponent<Arrow>();

        Transform breadOvenNavArrowTr = GameObject.Find("NavArrowOvenTr").GetComponent<Transform>();
        Transform breadDisplayAreaNavArrowTr = GameObject.Find("NavArrowDisplayTr").GetComponent<Transform>();
        Transform counterNavArrowTr = GameObject.Find("NavArrowCounterTr").GetComponent<Transform>();
        Transform counterMoneyNavArrowTr = GameObject.Find("CounterMoneyArrowTr").GetComponent<Transform>();
        Transform cafeteriaArrowTr = GameObject.Find("CafeteriaArrowTr").GetComponent<Transform>();

        AddNavArrowTransform("오븐", breadOvenNavArrowTr);
        AddNavArrowTransform("진열대", breadDisplayAreaNavArrowTr);
        AddNavArrowTransform("카운터", counterNavArrowTr);
        AddNavArrowTransform("카운터돈", counterMoneyNavArrowTr);
        AddNavArrowTransform("카페", cafeteriaArrowTr);


        //SetTargetPosition(arrow.currentArrowTarget);
    }
    void Update()
    {
        SetTargetPosition(arrow.currentArrowTarget);

        Vector3 directionToTarget = (targetPosition.position - playerTransform.position).normalized;
        directionToTarget.y = 0f;

        // 화살표의 위치를 캐릭터 기준 원형이 있는 것처럼 해당하는 위치로 배치
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
