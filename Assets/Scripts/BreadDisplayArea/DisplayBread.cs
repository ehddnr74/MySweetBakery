using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class DisplayBread : MonoBehaviour
{
    public List<Transform> breadTr = new List<Transform>();

    public int currentDisplayBreadCount;
    public int maxDisplayBreadCount;


    public int currentCustomerDisPlayAreaIndex;
    public int maxCustomerDisPlayAreaIndex = 3;

    public Stack<GameObject> displayBreadStack = new Stack<GameObject>();

    public List<bool> usingdisplayWayPoint = new List<bool>(); // 사용중인 자리인지

    private void Start()
    {
        for (int i=0;i<3;i++)
        {
            usingdisplayWayPoint.Add(false);
        }
    }
}


