using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public enum PlayerState
{
    DefaultIdle,
    DefaultMove,
    StackIdle,
    StackMove,
}

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public PlayerState playerState;
    public Cafeteria cafeteria;

    private ActivateBread activateBread;
    private DisplayBread displayBread;
    private Counter counter;
    private CounterStackMoney counterStackMoney;
    private CafeteriaStackMoney cafeteriaStackMoney;

    public TextMeshProUGUI maxCountText;
    public TextMeshProUGUI playerMoneyText;

    public Transform breadHolder;
    public Transform counterMoneyInteractionTr;
    public Transform cafeteriaMoneyInteractionTr;

    private Vector3 moveDirection;

    public int playerMoney;
    public int breadStackCurrentCount;
    public int breadStackMaxCount;

    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    /////////////// 다른 오브젝트와의 상호작용 거리 
    public float interactionPickUpDistance;
    public float interactionDropDistance;
    public float interactionCalculationDistance;
    public float interactionMoneyDistance;
    ///////////////

    public bool customerAblePickUp;

    private void Awake()
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

    private void Start()
    {
        GetGameObject();
        StartSetting();
    }

    private void FixedUpdate()
    {
        CalculateMoveDirection();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) // 오븐에서 구운 빵 줍기 
        {
            if (playerState == PlayerState.DefaultIdle || playerState == PlayerState.StackIdle)
            {
                PickUpBread();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) // 진열대에 빵 놓기
        {
            if (playerState == PlayerState.StackIdle)
            {
                DropBread();
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3)) // 돈 줍기
        {   
            if (CheckAblePickUpCounterMoney()) // 카운터 돈 회수 
            {
                if (counterStackMoney.stackMoneyCount > 0)
                {
                    PickUpCounterMoney();
                }
            }

            if (CheckAblePickUpCafeteriaMoney()) // 카페 돈 회수 
            {
                if (cafeteriaStackMoney.stackMoneyCount > 0)
                {
                    PickUpCafeteriaMoney();
                }
            }
        }

        CustomerAblePickUpCheck(); // 플레이어가 빵 진열대와 상호작용 거리를 벗어나야 고객이 빵을 담기 시작할 수 있게 구현 
    }

    private void GetGameObject()
    {
        activateBread = GameObject.Find("BreadOven").GetComponent<ActivateBread>();
        displayBread = GameObject.Find("BreadDisplayArea").GetComponent<DisplayBread>();
        counter = GameObject.Find("Counter").GetComponent<Counter>();
        counterStackMoney = GameObject.Find("CounterMoneyManager").GetComponent<CounterStackMoney>();
        cafeteriaStackMoney = GameObject.Find("CafeTeriaMoneyManager").GetComponent<CafeteriaStackMoney>();
    }
    private void StartSetting()
    {
        maxCountText.text = "";
    }

    private void CalculateMoveDirection()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector3(horizontal, 0, vertical).normalized;
    }

    public void PickUpBread()
    {
        if (breadStackCurrentCount >= breadStackMaxCount)
        {
            return;
        }

        Transform basketTr = GameObject.Find("BreadOvenBasket").GetComponent<Transform>();
        float distance = Vector3.Distance(basketTr.position, transform.position);

        if(distance <= interactionPickUpDistance)
        {
            if (activateBread.breadInBasket.Count > 0 && breadStackCurrentCount < breadStackMaxCount)
            {
                GameObject bread = activateBread.breadInBasket.Dequeue();
                activateBread.breadInPlayer.Push(bread);

                Rigidbody rb = bread.GetComponent<Rigidbody>();
                rb.useGravity = false;
                rb.isKinematic = true;
                bread.GetComponent<CapsuleCollider>().enabled = false;

                bread.transform.SetParent(breadHolder, true);
                bread.transform.localPosition = new Vector3(0f, 0f, breadStackCurrentCount * -0.36f);
                bread.transform.localRotation = Quaternion.Euler(-90f, -90f, 90f);
                SoundPool.instance.PlaySound("GetObject");
                //StartCoroutine(MoveAndRotateBread(bread, basketTr));

                breadStackCurrentCount++;
                if (breadStackCurrentCount == breadStackMaxCount)
                {
                    maxCountText.text = "MAX";
                }

                activateBread.currentCount--;
                if (!activateBread.isPlayingCoroutine)
                {
                    activateBread.ActiveBreadCoroutineStart();
                }
            }
        }
    }

    //private IEnumerator MoveAndRotateBread(GameObject bread, Transform startPos)
    //{
    //    Vector3 startPosition = startPos.position + Vector3.up;
    //    Vector3 endPosition = breadHolder.position + new Vector3(0f, breadStackCurrentCount * 0.36f, 0f);
    //    Quaternion startRotation = bread.transform.rotation;
    //    Quaternion endRotation = Quaternion.Euler(0f, 360f, 0f);

    //    float elapsedTime = 0f;
    //    float duration = 0.1f; 

    //    while (elapsedTime < duration)
    //    {
    //        float t = elapsedTime / duration;

    //        bread.transform.position = Vector3.Lerp(startPosition, endPosition, t);
    //        bread.transform.rotation = Quaternion.Lerp(startRotation, endRotation, t * 2f); 

    //        elapsedTime += Time.deltaTime;
    //        yield return null;
    //    }

    //    bread.transform.position = endPosition;
    //    bread.transform.rotation = endRotation;

    //    bread.transform.SetParent(breadHolder, true);
    //    bread.transform.localPosition = new Vector3(0f, 0f, breadStackCurrentCount * -0.36f);
    //    bread.transform.localRotation = Quaternion.Euler(-90f, -90f, 90f);

    //    yield return null;
    //}

    public void DropBread()
    {
        if (breadStackCurrentCount <= 0 || displayBread.currentDisplayBreadCount >= displayBread.maxDisplayBreadCount)
        {
            return;
        }

        float distance = Vector3.Distance(displayBread.transform.position, transform.position);

        if (distance <= interactionDropDistance)
        {
            if (breadStackCurrentCount > 0 && breadStackCurrentCount <= breadStackMaxCount)
            {
                GameObject bread = activateBread.breadInPlayer.Pop();
                displayBread.displayBreadStack.Push(bread);

                Transform breadTr = displayBread.breadTr[displayBread.displayBreadStack.Count - 1];

                //StartCoroutine(DropBreadAnimation(bread, breadTr));

                bread.transform.SetParent(breadTr, true);
                bread.transform.localScale = new Vector3(0.7f, 1f, 0.7f);
                bread.transform.localPosition = Vector3.zero;
                bread.transform.localRotation = Quaternion.identity;
                SoundPool.instance.PlaySound("PutObject");

                displayBread.currentDisplayBreadCount++;
                breadStackCurrentCount--;
                if (breadStackCurrentCount < breadStackMaxCount)
                {
                    maxCountText.text = "";
                }
            }
        }
    }

    //private IEnumerator DropBreadAnimation(GameObject bread, Transform targetTransform)
    //{
    //    float duration = 0.1f;
    //    float elapsedTime = 0f;

    //    bread.transform.localPosition = Vector3.zero;
    //    bread.transform.rotation = Quaternion.Euler(0f, 45f, 0f);

    //    Vector3 startPosition = targetTransform.position + new Vector3(0f,1.5f,0f);
    //    Vector3 targetPosition = targetTransform.position; 

    //    while (elapsedTime < duration)
    //    {
    //        float t = elapsedTime / duration;

    //        bread.transform.position = Vector3.Lerp(startPosition, targetPosition, t);

    //        elapsedTime += Time.deltaTime;
    //        yield return null;
    //    }

    //    bread.transform.position = targetPosition;

    //    bread.transform.SetParent(targetTransform, true);
    //    bread.transform.localScale = new Vector3(0.7f, 1f, 0.7f);

    //    yield return null;
    //}

    public bool CustomerAblePickUpCheck()
    {
        float distanceToDisplayBreadArea = Vector3.Distance(displayBread.transform.position, transform.position);

        if (distanceToDisplayBreadArea <= interactionDropDistance)
        {
            customerAblePickUp = false;
            return customerAblePickUp;
        }
        else
        {
            customerAblePickUp = true;
            return customerAblePickUp;
        }
    }

    public bool CustomerAbleCalCulation()
    {
        float distanceToCounter = Vector3.Distance(counter.transform.position, transform.position);
        if (distanceToCounter <= interactionCalculationDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool CheckAblePickUpCounterMoney()
    {
        float distance = Vector3.Distance(counterMoneyInteractionTr.position, transform.position);

        if(distance <= interactionMoneyDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private bool CheckAblePickUpCafeteriaMoney()
    {
        float distance = Vector3.Distance(cafeteriaMoneyInteractionTr.position, transform.position);

        if (distance <= interactionMoneyDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void PickUpCounterMoney()
    {
        StartCoroutine(GetCounterMoneyAnimation());
    }
    private IEnumerator GetCounterMoneyAnimation()
    {
        int totalMoneyCount = counterStackMoney.stackMoneyCount;

        for (int i = 0; i < totalMoneyCount; i++)
        {
            if (counterStackMoney.stackMoneyCount <= 0)
            {
                yield break;
            }

            counterStackMoney.RemoveMoney(1);
            AddMoney(1);

            GameObject moneyObj = PlayerMoney.instance.playerMoneyPool.GetObject();
            moneyObj.transform.position = transform.position + new Vector3(0f, 0f, 1f);

            Vector3 targetPosition = transform.position + 3 * (Vector3.up);

            StartCoroutine(CounterMoneyAbsorbing(moneyObj, targetPosition));

            yield return new WaitForSeconds(0.03f);
        }
    }

    private IEnumerator CounterMoneyAbsorbing(GameObject moneyObj, Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        float duration = 0.2f;

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

    public void PickUpCafeteriaMoney()
    {
        StartCoroutine(GetCafeteriaMoneyAnimation());
    }

    private IEnumerator GetCafeteriaMoneyAnimation()
    {
        int totalMoneyCount = cafeteriaStackMoney.stackMoneyCount;

        for (int i = 0; i < totalMoneyCount; i++)
        {
            if (cafeteriaStackMoney.stackMoneyCount <= 0)
            {
                yield break;
            }

            cafeteriaStackMoney.RemoveMoney(1);
            AddMoney(1);

            GameObject moneyObj = PlayerMoney.instance.playerMoneyPool.GetObject();
            moneyObj.transform.position = transform.position + new Vector3(0f, 0f, 1f);

            Vector3 targetPosition = transform.position + 3 * (Vector3.up);

            StartCoroutine(CafeteriaMoneyAbsorbing(moneyObj, targetPosition));

            yield return new WaitForSeconds(0.03f);
        }
    }

    private IEnumerator CafeteriaMoneyAbsorbing(GameObject moneyObj, Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        float duration = 0.2f;

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


    public void UpdateMoneyUI()
    {
        playerMoneyText.text = playerMoney.ToString();
    }

    public void AddMoney(int amount)
    {
        playerMoney += amount;
        UpdateMoneyUI();
    }

    public void RemoveMoney(int amount)
    {
        playerMoney -= amount;
        UpdateMoneyUI();
    }

    public Vector3 GetMoveDirection() { return moveDirection; }
}
