using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayBread : MonoBehaviour
{
    public List<Transform> breadTr = new List<Transform>();

    public int currentDisplayBreadCount;
    public int maxDisplayBreadCount;

    public Stack<GameObject> displayBreadStack = new Stack<GameObject>();
}
