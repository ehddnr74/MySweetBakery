using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class DisplayBread : MonoBehaviour
{

    public int currentDisplayBreadCount; // ���� �������� �� ���� 
    public int maxDisplayBreadCount; // ������ �� �ִ� �� �ִ� ���� 

    public int currentCustomerDisPlayAreaIndex; // ���� �����뿡 �� �ִ� �ڸ� Index (1��,2��,3�� �ڸ� ����)
    public int maxCustomerDisPlayAreaIndex = 3; // ���� �̿��� �� �ִ� �ִ� �ڸ��� 3����

    public Stack<GameObject> displayBreadStack = new Stack<GameObject>(); // �������� ���� �� �������� ���� 
    public List<Transform> breadTr = new List<Transform>(); // �������� ���� ���� ��ġ List�� ����
    public List<bool> usingdisplayWayPoint = new List<bool>(); // ���� �̿����� �ڸ����� ����,

    private void Start()
    {
        for (int i=0;i<3;i++)
        {
            usingdisplayWayPoint.Add(false);
        }
    }
}


