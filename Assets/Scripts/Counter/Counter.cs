using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : MonoBehaviour
{
    public int currentCounterIndex;
    public int maxCounterIndex = 3;

    public Queue<GameObject> counterCustomerQueue = new Queue<GameObject>();

    public List<bool> usingCounterWayPoint = new List<bool>(); // ������� �ڸ�����

    private void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            usingCounterWayPoint.Add(false);
        }
    }
}
