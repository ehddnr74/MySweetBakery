using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emoticon : MonoBehaviour
{
    public float rotationDuration = 1.0f; 
    public float scaleDuration = 1.0f;
    public float initialAngle = 0f;
    public float rotationAngle = 20f; 
    public Vector3 initialScale = Vector3.one;

    private void OnEnable()
    {
        // 초기 상태로 설정
        transform.localScale = initialScale;
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        // 애니메이션 시작
        StartCoroutine(EmoticonAnimation());
    }

    private IEnumerator EmoticonAnimation()
    {
        // 초기 스케일 저장
        Vector3 startScale = transform.localScale;

        // 회전: -30도에서 +30도로
        yield return StartCoroutine(RotateOverTime(initialAngle, -rotationAngle, rotationDuration));

        // 회전 후 대기
        yield return new WaitForSeconds(rotationDuration);

        // 회전: +30도에서 -30도로
        yield return StartCoroutine(RotateOverTime(-rotationAngle, rotationAngle, rotationDuration));

        // 스케일 줄어들면서 사라지기
        yield return StartCoroutine(ShrinkAndFade(startScale, scaleDuration));
    }

    private IEnumerator RotateOverTime(float startAngle, float endAngle, float duration)
    {
        float elapsed = 0f;
        Quaternion startRotation = Quaternion.Euler(0, 0, startAngle);
        Quaternion endRotation = Quaternion.Euler(0, 0, endAngle);

        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = endRotation;
    }

    private IEnumerator ShrinkAndFade(Vector3 startScale, float duration)
    {
        float elapsed = 0f;
        Vector3 endScale = Vector3.zero; // 최종 스케일은 (0,0,0)

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = endScale;

        // 완전히 사라진 후, 오브젝트 비활성화 (선택사항)
        gameObject.SetActive(false);
    }
}
