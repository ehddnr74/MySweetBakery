using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : MonoBehaviour
{
    public int currentCounterIndex; // 현재 카운터에 고객이 몇명 있는지 
    public int maxCounterIndex = 3; // 카운터에 설 수 있는 고객의 최대 수 

    public Queue<GameObject> counterCustomerQueue = new Queue<GameObject>(); //카운터에 고객이 지금 몇명 줄 서 있는지 Queue로 관리 

    public List<bool> usingCounterWayPoint = new List<bool>(); // 사용중인 자리인지

    private void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            usingCounterWayPoint.Add(false);
        }
    }
}
