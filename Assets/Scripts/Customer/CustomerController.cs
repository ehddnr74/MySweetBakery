using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum CustomerState
{
    MoveToEntrance,
    MoveToDisplayArea,
    ArriveToDisplayArea,
    MoveToCounter,
    ArriveToCounter,
    MoveToCounterForCafeteria,
    ArriveToCounterForCafeteria,
    Packaging,
    TurnToExit,
    GoToExit,
    TurnToDoor,
    GoToDoor,
    GoToCafeteria,
    ArriveToCafeteria,
    ExitToCafeteria,
    GoToDoorToCafeteria,
}

public class CustomerController : MonoBehaviour
{
    public CustomerState customerState;
    private DisplayBread displayBread;
    private Counter counter;
    private ObjectPool breadPool;
    public PaperBag paperBag;

    public GameObject emoticon;

    ///////////// ���� ������Ʈ�� ��� ��ġ 
    public Transform customerBreadHolder;
    public Transform customerPaperBagHolder;
    /////////////

    ///////////// ���� �ش��ϴ� ���(������Ʈ)�� �������� �� �ٶ������ Transform
    private Transform breadDisplayAreaTr;
    private Transform customerLookAtCounterTr;
    private Transform customerLookAtCafeteriaTr;


    ///////////// ���� �̵��ؾ��� �����
    public Transform startPoint;
    public Transform breadDisplayWayPoint;
    public Transform entranceWayPoint;
    public Transform counterWayPoint;
    public Transform counterForCafeteriaWayPoint;
    public Transform counterTurnToExitPoint;
    public Transform counterGoToExitPoint;
    public Transform turnToDoorPoint;
    public Transform goToDoorPoint;
    public Transform goToCafeteriaPoint;
    public Transform cafeteriaExitPoint;
    /////////////

    ///////////// �� �Ӹ��� ��ǳ��
    public GameObject window;
    public GameObject windowSingleIcon;
    public GameObject windowDualIcon;
    public TextMeshProUGUI windowText;
    /////////////

    public int currentStackBread;
    public int maxStackBread;

    public int spawnCount; // ���� ������ �� ���°�� ������ ������ 
    public int currentCounterLine; // ���� Coutner�� ��ġ�� �� ����� ���ο� �ִ���  

    public int currentDisplayWayPoint;
    public int currentCounterWayPoint;

    public bool enjoyTimeOverCafetria; // ī�� ���� �ð� �������� ���� 

    Coroutine pickUpCoroutine;

    public List<Transform> displayWayPoints = new List<Transform>(); // ���� ������� ���Ҷ� (1,2,3) �� �ڸ� �� �ϳ��� �������� ��
    public List<Transform> counterWayPoints = new List<Transform>(); // ���� ī���ͷ� ���Ҷ� (1,2,3) �� �ڸ� �� �ϳ��� �������� ��

    public Dictionary<string, Transform> objectTr = new Dictionary<string, Transform>(); // �Һ��ڰ� ������ �� ������Ʈ�� �ٶ󺸴� ������
                                                                                         // ȸ���ϱ� ���� ������Ʈ���� Transform ��� ��

    public Queue<GameObject> customerBreadStackQueue = new Queue<GameObject>(); // ���� �����뿡�� ���� ���� Queue�� ����
                                                                                // ī���Ϳ��� ������ �� ���� �������� ���� ������ ���� 
                                                                                // �Ųٷ��ҰŸ� �������� �ٲ� ���� �� 
    private void Awake()
    {
        GetAwakeGameObject();
        HideEmoticon();
    }

    private void OnEnable()
    {
        StartSetting();
    }
    private void OnDisable()
    {
        ResetSetting();
    }

    void Start()
    {
        GetGameObject();
        SetWayPoints();
        GetObjTransform();
        AddObjTransform(); // ���� �ٶ������ ������Ʈ�� �� Add�ϸ� �� (���� ������Ʈ �������� ȸ���ϱ� ����)

        HideWindow(); // �� �Ӹ� �� ��ǳ�� ����� 
    }
    private void GetAwakeGameObject()
    {
        breadPool = GameObject.Find("BreadPool").GetComponent<ObjectPool>();
        startPoint = GameObject.Find("CustomerStartPoint").transform;
    }

    private void GetObjTransform()
    {
        breadDisplayAreaTr = GameObject.Find("BreadDisplayArea").GetComponent<Transform>();
        customerLookAtCounterTr = GameObject.Find("CustomerLookAtCounterTr").GetComponent<Transform>();
        customerLookAtCafeteriaTr = GameObject.Find("CustomerLookAtCafeteriaTr").GetComponent<Transform>();
    }

    private void AddObjTransform()
    {
        objectTr.Add("������", breadDisplayAreaTr);
        objectTr.Add("ī����", customerLookAtCounterTr);
        objectTr.Add("ī��", customerLookAtCafeteriaTr);
    }

    public void PickUpBread()
    {
        if (currentStackBread == maxStackBread || pickUpCoroutine != null)
        {
            return;
        }

        if (counter.currentCounterIndex == counter.maxCounterIndex)
        {
            return;
        }

        pickUpCoroutine = StartCoroutine(PickUp());
    }

    private IEnumerator PickUp()
    {
        float randomPickUpStartTime = Random.Range(0.5f, 1.0f);

        yield return new WaitForSeconds(randomPickUpStartTime);

        while (currentStackBread < maxStackBread && displayBread.displayBreadStack.Count > 0)
        {
            GameObject bread = displayBread.displayBreadStack.Pop();
            displayBread.currentDisplayBreadCount--;

            customerBreadStackQueue.Enqueue(bread);

            bread.transform.SetParent(customerBreadHolder, true);
            bread.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            bread.transform.localPosition = new Vector3(0f, currentStackBread * 0.188f, 0f);
            bread.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
            SoundPool.instance.PlaySound("GetObject");

            currentStackBread++;

            WindowTextUpdate();

            yield return new WaitForSeconds(0.3f);
        }
        pickUpCoroutine = null;
    }


    public bool CustomerAblePickUp()
    {
        if (PlayerController.instance.CustomerAblePickUpCheck())
        {
            if (displayBread.displayBreadStack.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public void WindowSetting(bool dual, Sprite windowSprite)
    {
        if (spawnCount != 4)
        {
            if (dual)
            {
                window.SetActive(true);
                windowDualIcon.SetActive(true);
                windowSingleIcon.SetActive(false);
                windowDualIcon.GetComponent<Image>().sprite = windowSprite;
                int randomAmount = Random.Range(1, 4);
                windowText.text = randomAmount.ToString();
                maxStackBread = randomAmount;
            }
            else
            {
                window.SetActive(true);
                windowSingleIcon.SetActive(true);
                windowDualIcon.SetActive(false);
                windowSingleIcon.GetComponent<Image>().sprite = windowSprite;
            }
        }
        else
        {
            if (dual)
            {
                window.SetActive(true);
                windowDualIcon.SetActive(true);
                windowSingleIcon.SetActive(false);
                windowDualIcon.GetComponent<Image>().sprite = windowSprite;
                windowText.text = 1.ToString();
                maxStackBread = 1;
            }
            else
            {
                window.SetActive(true);
                windowSingleIcon.SetActive(true);
                windowDualIcon.SetActive(false);
                windowSingleIcon.GetComponent<Image>().sprite = windowSprite;
            }
        }
    }

    public void WindowTextUpdate()
    {
        int newTextAmount = maxStackBread - currentStackBread;
        windowText.text = newTextAmount.ToString();
    }

    public void HideWindow()
    {
        window.SetActive(false);
    }

    public void ShowEmoticon()
    {
        emoticon.SetActive(true);
    }
    public void HideEmoticon()
    {
        emoticon.SetActive(false);
    }

    private void GetGameObject()
    {
        displayBread = GameObject.Find("BreadDisplayArea").GetComponent<DisplayBread>();
        counter = GameObject.Find("Counter").GetComponent<Counter>();
    }

    private void SetWayPoints()
    {
        SetEntranceWayPoint();
        SetDisplayAreaWayPoints();
        SetCounterWayPoints();
        SetCounterCafeteriaWayPoints();
        SetCounterTurnToExitWayPoint();
        SetCounterGoToExitWayPoint();
        SetTurnToDoorWayPoint();
        SetGoToDoorWayPoint();
        SetGoToCafeteriaWayPoint();
        SetCafeteriaExitWayPoint();
    }

    private void SetEntranceWayPoint()
    {
        entranceWayPoint = GameObject.Find("EntrancePoint").transform;
    }
    private void SetDisplayAreaWayPoints()
    {
        Transform breadDisplayAreaPoint1 = GameObject.Find("DisPlayArea1").transform;
        Transform breadDisplayAreaPoint2 = GameObject.Find("DisPlayArea2").transform;
        Transform breadDisplayAreaPoint3 = GameObject.Find("DisPlayArea3").transform;

        displayWayPoints.Add(breadDisplayAreaPoint1);
        displayWayPoints.Add(breadDisplayAreaPoint2);
        displayWayPoints.Add(breadDisplayAreaPoint3);
    }

    public void SetDisplayAreaPoint()
    {
        for (int i = 0; i < displayBread.usingdisplayWayPoint.Count; i++)
        {
            if (displayBread.usingdisplayWayPoint[i] == false)
            {
                breadDisplayWayPoint = displayWayPoints[i];
                displayBread.usingdisplayWayPoint[i] = true;
                currentDisplayWayPoint = i;
                break;
            }
        }
    }

    private void SetCounterTurnToExitWayPoint()
    {
        counterTurnToExitPoint = GameObject.Find("CounterTurnToExitPoint").transform;
    }

    private void SetCounterGoToExitWayPoint()
    {
        counterGoToExitPoint = GameObject.Find("CounterGoToExitPoint").transform;
    }

    private void SetTurnToDoorWayPoint()
    {
        turnToDoorPoint = GameObject.Find("TurnToDoorPoint").transform;
    }

    private void SetGoToDoorWayPoint()
    {
        goToDoorPoint = GameObject.Find("GoToDoorPoint").transform;
    }

    private void SetGoToCafeteriaWayPoint()
    {
        goToCafeteriaPoint = GameObject.Find("GoToCafeteriaPoint").transform;
    }

    private void SetCafeteriaExitWayPoint()
    {
        cafeteriaExitPoint = GameObject.Find("CafeteriaExitTr").transform;
    }
    public void ReleaseDisplayWayPoint()
    {
        displayBread.usingdisplayWayPoint[currentDisplayWayPoint] = false;
    }

    public void SetCounterWayPoints()
    {
        Transform counterPoint1 = GameObject.Find("CounterPoint1").transform;
        Transform counterPoint2 = GameObject.Find("CounterPoint2").transform;
        Transform counterPoint3 = GameObject.Find("CounterPoint3").transform;

        counterWayPoints.Add(counterPoint1);
        counterWayPoints.Add(counterPoint2);
        counterWayPoints.Add(counterPoint3);
    }

    public void SetCounterPoint()
    {
        for (int i = 0; i < counter.usingCounterWayPoint.Count; i++)
        {
            if (counter.usingCounterWayPoint[i] == false)
            {
                counterWayPoint = counterWayPoints[i];
                counter.usingCounterWayPoint[i] = true;
                currentCounterWayPoint = i;
                currentCounterLine = i + 1;
                break;
            }
        }
    }

    public void SetCounterCafeteriaWayPoints()
    {
        counterForCafeteriaWayPoint = GameObject.Find("CounterCafeteriaPoint").transform;
    }

    public void ReleaseCounterWayPoint()
    {
        counter.usingCounterWayPoint[currentCounterWayPoint] = false;
    }

    public void SpawnCustomer(int amount)
    {
        CustomerSpawner.instance.GetObjectFromPool(amount);
    }

    public void AutoSpawnCustomer()
    {
        CustomerSpawner.instance.GetObjectsFromPool();
    }

    public bool CheckCounterFirstLineCount()
    {
        if (counter.usingCounterWayPoint[0] == false)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CheckCounterSecondLineCount()
    {
        if (counter.usingCounterWayPoint[1] == false)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void ResetSetting()
    {
        customerState = CustomerState.MoveToEntrance;
        currentStackBread = 0;
        currentDisplayWayPoint = 0;
        currentCounterLine = 0;

        window.SetActive(false);
    }
    private void StartSetting()
    {
        transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        transform.position = startPoint.position;
    }

    public void CafeteriaSettingBread(Transform tr)
    {
        GameObject obj = customerBreadHolder.transform.GetChild(0).gameObject;

        obj.transform.SetParent(tr.transform, false);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
        obj.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);

        StartCoroutine(CafeEnjoyTimeEnd(obj));
    }
    private IEnumerator CafeEnjoyTimeEnd(GameObject obj)
    {
        yield return new WaitForSeconds(8f);

        breadPool.ReturnObject(obj);

        if(PlayerController.instance.cafeteria.trash != null)
        {
            PlayerController.instance.cafeteria.trash.SetActive(true);

        }
       
        enjoyTimeOverCafetria = true;
    }
}
