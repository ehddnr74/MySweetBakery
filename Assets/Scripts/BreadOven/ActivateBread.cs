using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 오븐에서 빵을 생성하고 최대 바구니에 담을 수 있는 개수는 10개이다.
// 바구니의 빵이 10개이상이 되면 코루틴을 중지한다.

public class ActivateBread : MonoBehaviour
{
    public ObjectPool breadPool;

    public Transform activeBreadTransform;
    public Transform basketTransform;

    public int currentCount; // 현재 바구니의 빵 개수 
    public int maxCount; // 바구니에 담을 수 있는 빵 최대 개수 
    public float addForce = 5f; 

    public bool isPlayingCoroutine = false; // 코루틴 실행 여부 확인 플래그

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
