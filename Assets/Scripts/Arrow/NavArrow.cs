using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavArrow : MonoBehaviour
{
    private Arrow arrow;
    private Transform targetPosition; // Target = Arrow
    public Transform playerTransform;
    public float distance; // 플레이어와 화살표 거리 

    public Dictionary<string, Transform> navArrowTr = new Dictionary<string, Transform>();

    void Start()
    {
        GetGameObject();
        AddNavArrowTransform(); // NavArrow가 알아야 할 Transform들 여기서 추가  
    }
    void Update()
    {
        SetTargetPosition(arrow.currentArrowTarget); // Arrow의 Transform을 타겟으로 지정 
        SetNavArrowTransformUpdate(); // NavArrow 업데이트
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

        navArrowTr.Add("오븐", breadOvenNavArrowTr);
        navArrowTr.Add("진열대", breadDisplayAreaNavArrowTr);
        navArrowTr.Add("카운터", counterNavArrowTr);
        navArrowTr.Add("카운터돈", counterMoneyNavArrowTr);
        navArrowTr.Add("카페", cafeteriaNavArrowTr);
        navArrowTr.Add("두번째봉인지역", sealArea2NavArrowTr);
    }

    private void SetTargetPosition(string currentArrowTr)
    {
        targetPosition = navArrowTr[currentArrowTr];
    }
    private void SetNavArrowTransformUpdate()
    {
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
}
