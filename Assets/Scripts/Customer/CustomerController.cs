using System.Collections;
using System.Collections.Generic;
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
    Packaging,
    TurnToExit,
    GoToExit,
    TurnToDoor,
    GoToDoor,
}

public class CustomerController : MonoBehaviour
{
    public CustomerState customerState;


    public List<Transform> displayWayPoints = new List<Transform>();

    public List<Transform> counterWayPoints = new List<Transform>();

    public Dictionary<string, Transform> objectTr = new Dictionary<string, Transform>(); // 소비자가 도착한 후 오브젝트를 바라보는 각으로
                                                                                         // 회전하기 위해 오브젝트들의 Transform 담아 둠

    public Queue<GameObject> customerBreadStackQueue = new Queue<GameObject>();

    public Transform customerBreadHolder;
    public Transform customerPaperBagHolder;

    private Transform breadDisplayAreaTr;
    private Transform customerLookAtCounterTr;


    public Transform startPoint;
    public Transform breadDisplayWayPoint;
    public Transform entranceWayPoint;
    public Transform counterWayPoint;
    public Transform counterTurnToExitPoint;
    public Transform counterGoToExitPoint;
    public Transform turnToDoorPoint;
    public Transform goToDoorPoint;

    public GameObject window; // 말풍선
    public GameObject windowSingleIcon;
    public GameObject windowDualIcon;
    public TextMeshProUGUI windowText;

    private DisplayBread displayBread;
    private Counter counter;

    public int currentStackBread;
    public int maxStackBread;

    Coroutine pickUpCoroutine;

    public int currentDisplayWayPoint;
    public int currentCounterWayPoint;

    public int currentCounterLine;

    public PaperBag paperBag;

    public GameObject emoticon;

    private void Awake()
    {
        startPoint = GameObject.Find("CustomerStartPoint").transform;
        emoticon.SetActive(false);
    }

    private void OnEnable()
    {
        Starting();
    }
    private void OnDisable()
    {
        Reset();
    }

    void Start()
    {
        GetGameObject();
        SetWayPoints();
        GetObjTransform();

        // 소비자가 바라봐야할 오브젝트들 쭉 Add하면 됌 
        AddObjTransform("진열대", breadDisplayAreaTr); 
        AddObjTransform("카운터", customerLookAtCounterTr);

        window.SetActive(false);
    }

    private void GetObjTransform()
    {
        breadDisplayAreaTr = GameObject.Find("BreadDisplayArea").GetComponent<Transform>();
        customerLookAtCounterTr = GameObject.Find("CustomerLookAtCounterTr").GetComponent<Transform>();
    }

    private void AddObjTransform(string name, Transform tr)
    {
        objectTr.Add(name, tr);
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
        SetCounterTurnToExitWayPoint();
        SetCounterGoToExitWayPoint();
        SetTurnToDoorWayPoint();
        SetGoToDoorWayPoint();
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

    public void ReleaseCounterWayPoint()
    {
        counter.usingCounterWayPoint[currentCounterWayPoint] = false;
    }

    public void SpawnCustomer(int amount)
    {
        CustomerSpawner.instance.GetObjectFromPool(amount);
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
    private void Reset()
    {
        customerState = CustomerState.MoveToEntrance;
        currentStackBread = 0;
        currentDisplayWayPoint = 0;
        currentCounterLine = 0;

        window.SetActive(false);
    }
    private void Starting()
    {
        transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        transform.position = startPoint.position;
    }


}
