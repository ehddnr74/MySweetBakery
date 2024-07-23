using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform followTarget;  // ����ٴ� Ÿ�� (�÷��̾�)
    public float followSpeed = 10f; // ����ٴϴ� �ӵ�
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    private void Start()
    {
        // ī�޶� �ʱ� ��ġ
        transform.position = followTarget.position + offset;

        // Ŀ�� ���
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Vector3 targetPosition = followTarget.position + offset;

        //ĳ���Ͱ� �̵��Կ� ���� ī�޶��� ��ġ �̵� �������� (followSpeed ���� ����)
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed);
    }
}
