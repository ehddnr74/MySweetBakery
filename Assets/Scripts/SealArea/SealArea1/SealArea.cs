using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SealArea : MonoBehaviour
{
    public string sealAreaName; // 봉인존 이름 
    public int unLockInteractionDistance;

    public TextMeshProUGUI priceText;

    private int sealAreaPrice; // 봉인존 가격
    private bool unLock;

    public GameObject sealArea;
    public GameObject cafeteria;
   
    void Start()
    {
        GetSealAreaPrice();
    }
    void Update()
    {
        if (!unLock)
        { 
            UnLockAbleCheck();
        }
    }

    private void GetSealAreaPrice()
    {
        sealAreaPrice = GetPriceFromText();
    }

    private int GetPriceFromText()
    {
        int result;

        string text = priceText.text;

        if (int.TryParse(text, out result))
        {
            return result;
        }
        else
        {
            return 0;
        }
    }

    public void UnLockAbleCheck()
    {
        if (PlayerController.instance.playerMoney < sealAreaPrice)
        {
            return;
        }

        Transform playerTr = PlayerController.instance.transform;

        float distance = Vector3.Distance(transform.position, playerTr.position);
        
        if (distance <= unLockInteractionDistance)
        {
            unLock = true;

            StartCoroutine(GetMoneyToPlayer());
        }
    }

    private IEnumerator GetMoneyToPlayer()
    {
        while (sealAreaPrice > 0)
        {
            GameObject moneyObj = PlayerMoney.instance.playerMoneyPool.GetObject();
            moneyObj.transform.position = PlayerController.instance.transform.position + new Vector3(0f, 1.5f, 0f);

            StartCoroutine(MoneyAbsorbing(moneyObj, transform.position));

            PlayerController.instance.RemoveMoney(1);
            sealAreaPrice--;

            priceText.text = sealAreaPrice.ToString();

            yield return new WaitForSeconds(0.03f);
        }

        yield return new WaitForSeconds(0.5f);

        sealArea.SetActive(false);

        if (sealAreaName == "카페")
        {
            if (cafeteria != null)
            {
                cafeteria.SetActive(true);
                cafeteria.GetComponent<Cafeteria>().trash.SetActive(false);      
                EffectPool.instance.effectManager.ShowEffect("VFX_AppearSignStand", cafeteria.GetComponent<Cafeteria>().EffectTr.transform, Vector3.zero, Quaternion.identity, Vector3.one, 1.5f);
                SoundPool.instance.PlaySound("Success");
            }
        }

    }

    private IEnumerator MoneyAbsorbing(GameObject moneyObj, Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        float duration = 0.3f; 

        Vector3 startingPosition = moneyObj.transform.position;

        while (elapsedTime < duration)
        {
            moneyObj.transform.position = Vector3.Lerp(startingPosition, targetPosition, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        SoundPool.instance.PlaySound("GetObject");
        PlayerMoney.instance.playerMoneyPool.ReturnObject(moneyObj);
    }
}
