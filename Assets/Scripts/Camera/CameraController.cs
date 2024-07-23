using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform followTarget;  // 따라다닐 타겟 (플레이어)
    public float followSpeed = 10f; // 따라다니는 속도
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    private void Start()
    {
        // 카메라 초기 위치
        transform.position = followTarget.position + offset;

        // 커서 잠금
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Vector3 targetPosition = followTarget.position + offset;

        //캐릭터가 이동함에 따라 카메라의 위치 이동 선형보간 (followSpeed 변경 가능)
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed);
    }
}
