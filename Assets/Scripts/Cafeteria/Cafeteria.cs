using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cafeteria : MonoBehaviour
{
    private CameraController cameraController;

    public GameObject cafeteria;
    public GameObject sealArea2;
    public GameObject cafeteriaChair;
    public GameObject table;
    public GameObject trash;

    public Transform BreadTr;
    public Transform selaArea2CameraTargettingTr;
    public Transform EffectTr;

    private bool sealArea2TargettingFlag;
    public bool customerAbleCafeteria;

    void Start()
    {
        GetGameObject();
        StartSetting();
    }

    private void Update()
    {
        CheckAbleTargettingToSealArea2();
    }
    private void GetGameObject()
    {
        cameraController = GameObject.Find("Camera").GetComponent<CameraController>();
    }
    private void StartSetting()
    {
        cafeteria.SetActive(false);
    }
    private void CheckAbleTargettingToSealArea2()
    {
        if(sealArea2TargettingFlag)
        {
            return;
        }
        else
        {
            PlayerController.instance.cafeteria = this;
            customerAbleCafeteria = true;
            sealArea2TargettingFlag = true;
            sealArea2.SetActive(true);
            StartCoroutine(NewSealAreaTargetting());
        }  
    }

    private IEnumerator NewSealAreaTargetting()
    {
        yield return new WaitForSeconds(3.0f);

        cameraController.CameraOriginSetting();
        cameraController.CameraTargetting(selaArea2CameraTargettingTr);
    }
}
