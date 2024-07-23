using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PaperBag : MonoBehaviour
{
    public CustomerController customerController;
    public Animator mAnimator;
    private Coroutine containCoroutine;

    public Transform paperBagTr;

    private ObjectPool breadPool;

    public int maxContainCount;

    public float jumpHeight = 2.0f; 
    public float jumpDuration = 0.5f; 
    public float moveSpeed = 5.0f;
    public float rotationAngle = 90.0f;

    public bool finishContain;

    private void Awake()
    {
        breadPool = GameObject.Find("BreadPool").GetComponent<ObjectPool>();
        paperBagTr = GameObject.Find("PaperBagPosition").transform;
    }

    private void OnEnable()
    {
        if (mAnimator == null)
        {
            mAnimator = GetComponent<Animator>();
        }
        transform.localPosition = Vector3.zero;
        transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        transform.SetParent(paperBagTr, true);

        if (containCoroutine == null)
        {
            containCoroutine = StartCoroutine(PaperBagContain());
        }
    }
    private void OnDisable()
    {
        if (containCoroutine != null)
        {
            StopCoroutine(containCoroutine);
            containCoroutine = null;
        }
    }
    private IEnumerator PaperBagContain()
    {
        yield return new WaitForSeconds(1.0f);

        while (maxContainCount > 0)
        {
            GameObject bread = customerController.customerBreadStackQueue.Dequeue();
            breadPool.ReturnObject(bread);

            customerController.currentStackBread--;
            maxContainCount--;

            if (maxContainCount == 0)
            {
                yield return new WaitForSeconds(0.5f);

                //mAnimator.ResetTrigger("Appear");
                //mAnimator.SetTrigger("Close");
                mAnimator.Play("Paper Bag_close", 0, 0f);
                StopCoroutine(containCoroutine);
                containCoroutine = null;
                PickUpPaperBag();
            }

            yield return new WaitForSeconds(0.5f);
            mAnimator.ResetTrigger("Close");
        }
    }

    public void PickUpPaperBag()
    {
        if (containCoroutine == null)
        {
            containCoroutine = StartCoroutine(PickUpPaperBagCoroutine());
        }
    }
    private IEnumerator PickUpPaperBagCoroutine()
    {
        Vector3 startPosition = transform.position;
        Vector3 holdPosition = customerController.customerPaperBagHolder.position;
        Vector3 midPoint = (startPosition + holdPosition) / 2;
        midPoint.y += jumpHeight; // ���� ���� �߰�

        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(startRotation.eulerAngles + new Vector3(0, rotationAngle, 0)); // Y������ ȸ��

        float startTime = Time.time;
        float journeyLength = Vector3.Distance(startPosition, holdPosition);

        // ���� �ִϸ��̼� (��θ� ���� �̵�)
        while (Time.time - startTime < jumpDuration)
        {
            float t = (Time.time - startTime) / jumpDuration;
            // ���ڰ� ��� ���� �̵��ϵ��� ����
            transform.position = Vector3.Lerp(Vector3.Lerp(startPosition, midPoint, t), Vector3.Lerp(midPoint, holdPosition, t), t);
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);
            yield return null;
        }

        // ������ �Ϸ�� �� ���� ǰ���� ������ �̵�
        float moveStartTime = Time.time;
        while (Vector3.Distance(transform.position, holdPosition) > 0.1f)
        {
            float moveT = (Time.time - moveStartTime) * moveSpeed / journeyLength;
            transform.position = Vector3.Lerp(transform.position, holdPosition, moveT);
            transform.rotation = Quaternion.Lerp(endRotation, endRotation, moveT);
            yield return null;
        }

        transform.SetParent(customerController.customerPaperBagHolder, false);
        // ��ġ ����
        transform.position = holdPosition;
        transform.rotation = endRotation;

        yield return new WaitForSeconds(0.5f);
        finishContain = true;
    }
}
