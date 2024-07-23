using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    public static CustomerSpawner instance;

    public ObjectPool customerPool;
    private DisplayBread displayBread;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        GetCustomerPool();
        GetObjectFromPool(3); // 인자 = 몇명 소환할건지 
    }


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            GetObjectFromPool(1); // 인자 = 몇명 소환할건지 
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            GetObjectFromPool(2); // 인자 = 몇명 소환할건지 
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            GetObjectFromPool(3); // 인자 = 몇명 소환할건지 
        }
    }

    private void GetCustomerPool()
    {
        customerPool = GameObject.Find("CustomerManager").GetComponent<ObjectPool>();
        displayBread = GameObject.Find("BreadDisplayArea").GetComponent<DisplayBread>();
    }

    public void GetObjectFromPool(int amount)
    {
        if (displayBread.currentCustomerDisPlayAreaIndex + amount > displayBread.maxCustomerDisPlayAreaIndex)
        {
            return;
        }
        if (displayBread.currentCustomerDisPlayAreaIndex < displayBread.maxCustomerDisPlayAreaIndex)
        {
            StartCoroutine(GetCustomerFromPool(amount));
        }    
    }

    private IEnumerator GetCustomerFromPool(int amount)
    {
        for(int i=0;i<amount;i++)
        {
            displayBread.currentCustomerDisPlayAreaIndex++;
            customerPool.GetObject();

            float randomWaitTime = Random.Range(0.8f, 1.0f);
            yield return new WaitForSeconds(randomWaitTime);
        }
    }
}
