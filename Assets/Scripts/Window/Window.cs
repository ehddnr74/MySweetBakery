using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(WindowAnimation());
    }

    private IEnumerator WindowAnimation()
    {
        float duration = 0.2f;

        Vector3 startScale1 = new Vector3(0.7f, 0.7f, 1f);
        Vector3 endScale1 = new Vector3(0.9f, 0.9f, 1f);
        yield return ScaleOverTime(startScale1, endScale1, duration);

        Vector3 startScale2 = new Vector3(0.8f, 0.9f, 1f);
        Vector3 endScale2 = new Vector3(0.9f, 1.2f, 1f);
        yield return ScaleOverTime(startScale2, endScale2, duration);

        Vector3 startScale3 = new Vector3(0.7f, 1.3f, 1f);
        Vector3 endScale3 = new Vector3(1f, 1f, 1f);
        yield return ScaleOverTime(startScale3, endScale3, duration);
    }

    private IEnumerator ScaleOverTime(Vector3 startScale, Vector3 endScale, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = endScale;
    }
}
