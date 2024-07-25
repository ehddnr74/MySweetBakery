using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterStackMoney : MonoBehaviour
{
    public ObjectPool moneyPool;
    public Vector3 startPosition; // 시작위치 인스펙터창에서 바꿔서 사용

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
        SetMoneyPositions(); // 돈이 생성될 위치들 싹 다 큐에 넣어놈
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

    public void SetMoneyPositions() // 행,열,층 순서로 큐에 넣음
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

    public void SpawnMoney(int amount) // 생성될 때 큐에서 꺼내서 후에 제거로직을 위해 스택에 넣음
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

    public void RemoveMoney(int amount) // 스택을 사용했기 때문에 가장 마지막에 생성된 돈부터 제거됨
    {                          // 제거하고 나서 제거된 위치에 돈이 다시 생성되게 하기 위해
                               // 임시큐를 만들고 기존의 위치들 저장 이후 기존 큐의 첫번째 원소에 제거된 돈 위치 넣어주고 
                               // 그 다음 원소들을 기존의 위치들로 채움 
                               // ** 한번 제거할 때마다 꼭 while문을 통해 Queue 정렬해줘야 순서대로 보장됨 
                               // 순서 보장필요없으면 임시Queue를 for문 전에 넣어놓고 for문 끝나면 while문 돌려주면 됨

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
