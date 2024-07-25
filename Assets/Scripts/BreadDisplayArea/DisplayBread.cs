using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class DisplayBread : MonoBehaviour
{

    public int currentDisplayBreadCount; // 현재 진열대의 빵 개수 
    public int maxDisplayBreadCount; // 진열할 수 있는 빵 최대 개수 

    public int currentCustomerDisPlayAreaIndex; // 고객이 진열대에 서 있는 자리 Index (1번,2번,3번 자리 있음)
    public int maxCustomerDisPlayAreaIndex = 3; // 고객이 이용할 수 있는 최대 자리는 3군데

    public Stack<GameObject> displayBreadStack = new Stack<GameObject>(); // 진열대의 쌓인 빵 스택으로 관리 
    public List<Transform> breadTr = new List<Transform>(); // 진열대의 빵이 놓일 위치 List로 보관
    public List<bool> usingdisplayWayPoint = new List<bool>(); // 고객이 이용중인 자리인지 여부,

    private void Start()
    {
        for (int i=0;i<3;i++)
        {
            usingdisplayWayPoint.Add(false);
        }
    }
}


