using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperBagManager : MonoBehaviour
{
    public static PaperBagManager instance;

    public ObjectPool paperBagPool;

    public Transform paperBagTr;

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
        GetGameObject();
    }

    public void GetObjectFromPool(CustomerController customerController, int amount)
    {
        GameObject paperBag = paperBagPool.GetObject();
        paperBag.SetActive(false);
        customerController.paperBag = paperBag.GetComponent<PaperBag>();
        paperBag.GetComponent<PaperBag>().customerController = customerController;
        paperBag.GetComponent<PaperBag>().maxContainCount = amount;

        paperBag.SetActive(true);
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.transform.localPosition = Vector3.zero;
        obj.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        obj.transform.SetParent(paperBagTr, true);
        paperBagPool.ReturnObject(obj);
    }

    private void GetGameObject()
    {
        paperBagPool = GameObject.Find("PaperBagManager").GetComponent<ObjectPool>();
    }
}
