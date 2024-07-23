using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateBread : MonoBehaviour
{
    public Transform activeBreadTransform;
    public ObjectPool breadPool;

    public Transform basketTransform;
    public float addForce = 5f; 

    public int currentCount;
    public int maxCount;

    public Queue<GameObject> breadInBasket = new Queue<GameObject>();
    public Stack<GameObject> breadInPlayer = new Stack<GameObject>();

    public bool isPlayingCoroutine = false; // 코루틴 실행 여부 확인 플래그

    void Start()
    {  
        breadPool = GameObject.Find("BreadPool").GetComponent<ObjectPool>();
        ActiveBreadCoroutineStart();
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
            
            bread.GetComponent<Rigidbody>().useGravity = false;
            bread.GetComponent<Rigidbody>().isKinematic = false;

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
