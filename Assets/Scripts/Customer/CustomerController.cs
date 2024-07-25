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

    ///////////// 고객이 오브젝트를 잡는 위치 
    public Transform customerBreadHolder;
    public Transform customerPaperBagHolder;
    /////////////

    ///////////// 고객이 해당하는 장소(오브젝트)에 도착했을 때 바라봐야할 Transform
    private Transform breadDisplayAreaTr;
    private Transform customerLookAtCounterTr;
    private Transform customerLookAtCafeteriaTr;


    ///////////// 고객이 이동해야할 방향들
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

    ///////////// 고객 머리위 말풍선
    public GameObject window;
    public GameObject windowSingleIcon;
    public GameObject windowDualIcon;
    public TextMeshProUGUI windowText;
    /////////////

    public int currentStackBread;
    public int maxStackBread;

    public int spawnCount; // 고객이 생성될 때 몇번째로 생성된 고객인지 
    public int currentCounterLine; // 고객이 Coutner에 위치할 때 몇번쨰 라인에 있는지  

    public int currentDisplayWayPoint;
    public int currentCounterWayPoint;

    public bool enjoyTimeOverCafetria; // 카페 즐기는 시간 끝났는지 여부 

    Coroutine pickUpCoroutine;

    public List<Transform> displayWayPoints = new List<Transform>(); // 고객은 진열대로 향할때 (1,2,3) 번 자리 중 하나로 정해져서 들어감
    public List<Transform> counterWayPoints = new List<Transform>(); // 고객은 카운터로 향할때 (1,2,3) 번 자리 중 하나로 정해져서 들어감

    public Dictionary<string, Transform> objectTr = new Dictionary<string, Transform>(); // 소비자가 도착한 후 오브젝트를 바라보는 각으로
                                                                                         // 회전하기 위해 오브젝트들의 Transform 담아 둠

    public Queue<GameObject> customerBreadStackQueue = new Queue<GameObject>(); // 고객이 진열대에서 담은 빵을 Queue로 담음
                                                                                // 카운터에서 포장할 때 먼저 담은것을 먼저 빼내기 위함 
                                                                                // 거꾸로할거면 스택으로 바꿔 쓰면 됨 
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
        AddObjTransform(); // 고객이 바라봐야할 오브젝트들 쭉 Add하면 됌 (고객이 오브젝트 방향으로 회전하기 위함)

        HideWindow(); // 고객 머리 위 말풍선 숨기기 
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
        objectTr.Add("진열대", breadDisplayAreaTr);
        objectTr.Add("카운터", customerLookAtCounterTr);
        objectTr.Add("카페", customerLookAtCafeteriaTr);
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
