using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ���쿡�� ���� �����ϰ� �ִ� �ٱ��Ͽ� ���� �� �ִ� ������ 10���̴�.
// �ٱ����� ���� 10���̻��� �Ǹ� �ڷ�ƾ�� �����Ѵ�.

public class ActivateBread : MonoBehaviour
{
    public ObjectPool breadPool;

    public Transform activeBreadTransform;
    public Transform basketTransform;

    public int currentCount; // ���� �ٱ����� �� ���� 
    public int maxCount; // �ٱ��Ͽ� ���� �� �ִ� �� �ִ� ���� 
    public float addForce = 5f; 

    public bool isPlayingCoroutine = false; // �ڷ�ƾ ���� ���� Ȯ�� �÷���

    public Queue<GameObject> breadInBasket = new Queue<GameObject>();
    public Stack<GameObject> breadInPlayer = new Stack<GameObject>(); 


    void Start()
    {
        GetGameObject();
        ActiveBreadCoroutineStart();
    }
    private void GetGameObject()
    {
        breadPool = GameObject.Find("BreadPool").GetComponent<ObjectPool>();
    }
    public void ActiveBreadCoroutineStart()
    {
        StartCoroutine(ActiveBread());
    }

    private IEnumerator ActiveBread()
    {
        isPlayingCoroutine = true;

        while (currentCount < maxCount)
        {
            GameObject bread = breadPool.GetObject();
            bread.transform.SetParent(activeBreadTransform, false);
            bread.transform.localPosition = Vector3.zero;
            bread.transform.localRotation = Quaternion.identity;
            bread.transform.localScale = Vector3.one;
            
            bread.GetComponent<Rigidbody>().useGravity = false;
            bread.GetComponent<Rigidbody>().isKinematic = false;
            bread.GetComponent<CapsuleCollider>().enabled = true;

            StartCoroutine(ToFallBasket(bread));

            currentCount++;

            yield return new WaitForSeconds(1.5f);
            breadInBasket.Enqueue(bread);
        }

        isPlayingCoroutine = false;
    }

    private IEnumerator ToFallBasket(GameObject bread)
    {
        yield return new WaitForSeconds(0.5f);

        Rigidbody rb = bread.GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.AddForce(new Vector3(0f,0f,-addForce), ForceMode.VelocityChange);
    }
}
