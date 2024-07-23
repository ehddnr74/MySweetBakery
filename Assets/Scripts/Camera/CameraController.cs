using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform followTarget;  // ����ٴ� Ÿ�� (�÷��̾�)
    public float followSpeed = 10f; // ����ٴϴ� �ӵ�
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    private bool targeting; 
    Vector3 originPos;
    Quaternion originRot;

    private void Start()
    {
        // ī�޶� �ʱ� ��ġ
        transform.position = followTarget.position + offset;

        // Ŀ�� ���
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (!targeting)
        {
            Vector3 targetPosition = followTarget.position + offset;

            //ĳ���Ͱ� �̵��Կ� ���� ī�޶��� ��ġ �̵� �������� (followSpeed ���� ����)
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed);
        }
    }

    public void CameraTargetting(Transform target, float camSpeed = 0.05f)
    {
        if (target != null)
        {
            StopAllCoroutines();
            targeting = true;
            StartCoroutine(CameraTargettingCoroutine(target, camSpeed));
        }
    }

    public void CameraReset()
    {
        StopAllCoroutines();
        StartCoroutine(CameraResetCoroutine());
    }

    public void CameraOriginSetting()
    {
        originPos = transform.position;
        originRot = transform.rotation;
    }

    IEnumerator CameraTargettingCoroutine(Transform target, float camSpeed = 0.005f)
    {
        Vector3 targetPos = target.position;
        targetPos.y = originPos.y;

        while (Vector3.Distance(transform.position, targetPos) > 0.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, camSpeed);
            yield return null;
        }

        yield return new WaitForSeconds(2.0f);
        StartCoroutine(CameraResetCoroutine());
    }

    IEnumerator CameraResetCoroutine(float camSpeed = 0.05f)
    {
        while (Vector3.Distance(transform.position, originPos) > 0.2f)
        {
            transform.position = Vector3.MoveTowards(transform.position, originPos, camSpeed);
            yield return null;
        }
        transform.position = originPos;
        targeting = false;
    }
}
