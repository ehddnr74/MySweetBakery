using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform followTarget;  // 따라다닐 타겟 (플레이어)
    public float followSpeed = 10f; // 따라다니는 속도
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    private bool targeting; // 다른곳을 타게팅하고있는지 여부 변수
    Vector3 originPos;

    private void Start()
    {
        TransformSetting();
        LockCursor();
    }

    private void Update()
    {
        if(!targeting)
        {
            CameraUpdate();
        }
    }

    private void TransformSetting()
    {
        transform.position = followTarget.position + offset;
    }
    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void CameraUpdate()
    {
        Vector3 targetPosition = followTarget.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed);
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
