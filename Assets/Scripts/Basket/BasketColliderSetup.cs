using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketColliderSetup : MonoBehaviour
{
    public GameObject basket;
    void Start()
    {
        SetupColliders();
    }

    void SetupColliders()
    {
        AddBoxCollider(new Vector3(0, 0.5f, 0.9f), new Vector3(2f, 1.2f, 0.2f)); // �ո�
        AddBoxCollider(new Vector3(0, 0.5f, -0.9f), new Vector3(2f, 1.2f, 0.2f)); // �޸�
        AddBoxCollider(new Vector3(-0.9f, 0.5f, 0f), new Vector3(0.2f, 1.2f, 2f)); // ���� ��
        AddBoxCollider(new Vector3(0.9f, 0.5f, 0f), new Vector3(0.2f, 1.2f, 2f)); // ������ ��
    }

    void AddBoxCollider(Vector3 center, Vector3 size)
    {
        BoxCollider boxCollider = basket.AddComponent<BoxCollider>();
        boxCollider.center = center;
        boxCollider.size = size;
    }
}
