using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterStackMoney : MonoBehaviour
{
    public ObjectPool moneyPool;
    public Vector3 startPosition; // ������ġ �ν�����â���� �ٲ㼭 ���

    public int rows;
    public int columns;
    public int layers;

    public float xSpacing;
    public float ySpacing;
    public float zSpacing;

    private Stack<GameObject> stackMoney = new Stack<GameObject>();
    private Queue<Vector3> emptyMoneySpace = new Queue<Vector3>();

    public int stackMoneyCount;

    private Transform sealAreaTr;
    private CameraController cameraController;
    public bool cameraZoomInFlag;

    private void Start()
    {
        cameraController = GameObject.Find("Camera").GetComponent<CameraController>();
        sealAreaTr = GameObject.Find("SealAreaCameraTargettingTr").transform;
        SetMoneyPositions(); // ���� ������ ��ġ�� �� �� ť�� �־��
    }
    private void Update()
    {
        if (stackMoneyCount >= 30 && !cameraZoomInFlag)
        {
            cameraZoomInFlag = true;
            cameraController.CameraOriginSetting();
            cameraController.CameraTargetting(sealAreaTr);
        }

    }

    public void SetMoneyPositions() // ��,��,�� ������ ť�� ����
    {
        emptyMoneySpace.Clear();

        for (int layer = 0; layer < layers; layer++)
        {
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    Vector3 position = startPosition + new Vector3(column * xSpacing, layer * ySpacing, row * zSpacing);
                    emptyMoneySpace.Enqueue(position);
                }
            }
        }
    }

    public void SpawnMoney(int amount) // ������ �� ť���� ������ �Ŀ� ���ŷ����� ���� ���ÿ� ����
    {
        for (int i = 0; i < amount; i++)
        {
            if (emptyMoneySpace.Count == 0)
            {
                break;
            }

            Vector3 position = emptyMoneySpace.Dequeue();
            GameObject money = moneyPool.GetObject();
            money.transform.position = position;

            stackMoney.Push(money);
            stackMoneyCount++;
        }
    }

    public void RemoveMoney(int amount) // ������ ����߱� ������ ���� �������� ������ ������ ���ŵ�
    {                          // �����ϰ� ���� ���ŵ� ��ġ�� ���� �ٽ� �����ǰ� �ϱ� ����
                               // �ӽ�ť�� ����� ������ ��ġ�� ���� ���� ���� ť�� ù��° ���ҿ� ���ŵ� �� ��ġ �־��ְ� 
                               // �� ���� ���ҵ��� ������ ��ġ��� ä�� 
                               // ** �ѹ� ������ ������ �� while���� ���� Queue ��������� ������� ����� 
                               // ���� �����ʿ������ �ӽ�Queue�� for�� ���� �־���� for�� ������ while�� �����ָ� ��

        for (int i = 0; i < amount; i++)
        {
            if (stackMoney.Count == 0)
            {
                return;
            }
            GameObject money = stackMoney.Pop();
            Vector3 position = money.transform.position;
            moneyPool.ReturnObject(money);

            var tempQueue = new Queue<Vector3>(emptyMoneySpace);
            emptyMoneySpace.Clear();
            emptyMoneySpace.Enqueue(position);

            while (tempQueue.Count > 0)
            {
                emptyMoneySpace.Enqueue(tempQueue.Dequeue());
            }

            stackMoneyCount--;
        }
    }

    public void RemoveMoneyAll()
    {
        int stackCount = stackMoney.Count;
        for (int i = 0; i < stackCount; i++)
        {
            if (stackMoney.Count == 0)
            {
                break;
            }

            GameObject money = stackMoney.Pop();
            Vector3 position = money.transform.position;
            moneyPool.ReturnObject(money);

            var tempQueue = new Queue<Vector3>(emptyMoneySpace);
            emptyMoneySpace.Clear();
            emptyMoneySpace.Enqueue(position);

            while (tempQueue.Count > 0)
            {
                emptyMoneySpace.Enqueue(tempQueue.Dequeue());
            }

            stackMoneyCount--;
            PlayerController.instance.AddMoney(1);
        }
    }
}
