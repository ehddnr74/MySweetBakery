using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour
{
    Arrow arrow;

    public float upDuration = 0.8f; 
    public float fallDuration = 0.5f; 

    private bool isInteracting;

    private void Start()
    {
        GetGameObject();
    }
    private void Update()
    {
        if (AbleInteractionCheck() && !isInteracting) 
        {
            isInteracting = true;
            StartCoroutine(ToPlayerAnimation());
        }
    }

    private void GetGameObject()
    {
        arrow = GameObject.Find("Arrow").GetComponent<Arrow>();
    }

    private IEnumerator ToPlayerAnimation()
    {
        Vector3 initialPosition = transform.position;
        Vector3 targetPosition = PlayerController.instance.transform.position;
        targetPosition.y = initialPosition.y; 

        float elapsedTime = 0f;
        while (elapsedTime < upDuration)
        {
            transform.position = Vector3.Lerp(initialPosition, initialPosition + (Vector3.up), elapsedTime / upDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = initialPosition + Vector3.up; 

        elapsedTime = 0f;
        Vector3 midPosition = transform.position;
        while (elapsedTime < fallDuration)
        {
            transform.position = Vector3.Lerp(midPosition, targetPosition, elapsedTime / fallDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition; 

        EffectPool.instance.effectManager.ShowEffect("VFX_Clean", PlayerController.instance.cafeteria.EffectTr, Vector3.zero, Quaternion.identity, new Vector3(10f, 10f, 10f), 1.0f);
        SoundPool.instance.PlaySound("Trash");
        gameObject.SetActive(false);
        arrow.ableSealArea2Target = true;
    }
    private bool AbleInteractionCheck()
    {
        float distance = Vector3.Distance(PlayerController.instance.transform.position, transform.position);

        if (distance <= PlayerController.instance.interactionPickUpDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
