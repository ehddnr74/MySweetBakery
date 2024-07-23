using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform followTarget;  // 따라다닐 타겟 (플레이어)
    public float followSpeed = 10f; // 따라다니는 속도
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    private bool targeting; 
    Vector3 originPos;
    Quaternion originRot;

    private void Start()
    {
        // 카메라 초기 위치
        transform.position = followTarget.position + offset;

        // 커서 잠금
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (!targeting)
        {
            Vector3 targetPosition = followTarget.position + offset;

            //캐릭터가 이동함에 따라 카메라의 위치 이동 선형보간 (followSpeed 변경 가능)
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
