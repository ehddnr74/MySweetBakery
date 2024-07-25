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

    public int customerSpawnCount; // 고객이 생성될 때 몇번째로 생성된 고객인지 CustomerController에 넣어줌 
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
        GetObjectFromPool(2); // 인자 = 몇명 소환할건지 
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

    public void GetObjectsFromPool()
    {
        if (displayBread.currentCustomerDisPlayAreaIndex == displayBread.maxCustomerDisPlayAreaIndex)
        {
            return;
        }

        StartCoroutine(GetCustomersFromPool());
    }

    private IEnumerator GetCustomerFromPool(int amount)
    {
        for(int i=0;i<amount;i++)
        {
            customerSpawnCount++;
            displayBread.currentCustomerDisPlayAreaIndex++;
            GameObject customer = customerPool.GetObject();
            customer.GetComponent<CustomerController>().spawnCount = customerSpawnCount;

            float randomWaitTime = Random.Range(0.8f, 1.0f);
            yield return new WaitForSeconds(randomWaitTime);
        }
    }

    private IEnumerator GetCustomersFromPool()
    {
        while (displayBread.currentCustomerDisPlayAreaIndex < displayBread.maxCustomerDisPlayAreaIndex)
        {
            customerSpawnCount++;
            displayBread.currentCustomerDisPlayAreaIndex++;
            GameObject customer = customerPool.GetObject();
            customer.GetComponent<CustomerController>().spawnCount = customerSpawnCount;

            float randomWaitTime = Random.Range(1.0f, 1.5f);
            yield return new WaitForSeconds(randomWaitTime);
        }
    }
}
