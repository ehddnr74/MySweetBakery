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
    private ObjectPool breadPool;

    public Transform paperBagTr;

    public int maxContainCount;
    public int moneyCount; // 고객이 빵을 몇개 담았는지에 따라 moneyCount가 달라지며 moneyCount에 맞는 수량의 돈을 풀에서 활성화시킴

    public float jumpHeight = 2.0f; 
    public float jumpDuration = 0.5f; 
    public float moveSpeed = 5.0f;
    public float rotationAngle = 90.0f;

    public bool finishContain; // 고객이 보유한 빵을 모두 상자에 담았는지 여부 


    private void Awake()
    {
        GetGameObject();
    }
    private void OnEnable()
    {
        StartSetting();
    }
    private void OnDisable()
    {
        StopCoroutine();
    }

    private void GetGameObject()
    {
        breadPool = GameObject.Find("BreadPool").GetComponent<ObjectPool>();
        paperBagTr = GameObject.Find("PaperBagPosition").transform;
    }
    private void StartSetting()
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
    private void StopCoroutine()
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
            moneyCount++;

            if (maxContainCount == 0)
            {
                yield return new WaitForSeconds(0.5f);
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
        midPoint.y += jumpHeight; 

        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(startRotation.eulerAngles + new Vector3(0, rotationAngle, 0));

        float startTime = Time.time;
        float journeyLength = Vector3.Distance(startPosition, holdPosition);

        while (Time.time - startTime < jumpDuration)
        {
            float t = (Time.time - startTime) / jumpDuration;
            transform.position = Vector3.Lerp(Vector3.Lerp(startPosition, midPoint, t), Vector3.Lerp(midPoint, holdPosition, t), t);
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);
            yield return null;
        }

        float moveStartTime = Time.time;
        while (Vector3.Distance(transform.position, holdPosition) > 0.1f)
        {
            float moveT = (Time.time - moveStartTime) * moveSpeed / journeyLength;
            transform.position = Vector3.Lerp(transform.position, holdPosition, moveT);
            transform.rotation = Quaternion.Lerp(endRotation, endRotation, moveT);
            yield return null;
        }

        transform.SetParent(customerController.customerPaperBagHolder, false);
        transform.position = holdPosition;
        transform.rotation = endRotation;

        yield return new WaitForSeconds(0.5f);
        finishContain = true;
    }
}
