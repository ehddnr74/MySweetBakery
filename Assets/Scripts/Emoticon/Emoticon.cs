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
        // �ʱ� ���·� ����
        transform.localScale = initialScale;
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        // �ִϸ��̼� ����
        StartCoroutine(EmoticonAnimation());
    }

    private IEnumerator EmoticonAnimation()
    {
        // �ʱ� ������ ����
        Vector3 startScale = transform.localScale;

        // ȸ��: -30������ +30����
        yield return StartCoroutine(RotateOverTime(initialAngle, -rotationAngle, rotationDuration));

        // ȸ�� �� ���
        yield return new WaitForSeconds(rotationDuration);

        // ȸ��: +30������ -30����
        yield return StartCoroutine(RotateOverTime(-rotationAngle, rotationAngle, rotationDuration));

        // ������ �پ��鼭 �������
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
        Vector3 endScale = Vector3.zero; // ���� �������� (0,0,0)

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = endScale;

        // ������ ����� ��, ������Ʈ ��Ȱ��ȭ (���û���)
        gameObject.SetActive(false);
    }
}
