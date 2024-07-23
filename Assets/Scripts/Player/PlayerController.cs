using System.Collections;
using System.Collections.Generic;
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
    public PlayerState playerState;

    public Transform breadHolder;

    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    private Vector3 moveDirection;
    private ActivateBread activateBread;
    private DisplayBread displayBread;


    public int breadStackMaxCount;
    public int breadStackCurrentCount;

    public TextMeshProUGUI maxCountText;

    private void Start()
    {
        activateBread = GameObject.Find("BreadOven").GetComponent<ActivateBread>();
        displayBread = GameObject.Find("BreadDisplayArea").GetComponent<DisplayBread>();
        maxCountText.text = "";
    }

    private void FixedUpdate()
    {
        CalculateMoveDirection();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (playerState == PlayerState.DefaultIdle || playerState == PlayerState.StackIdle)
            {
                PickUpBread();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (playerState == PlayerState.StackIdle)
            {
                DropBread();
            }
        }
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

        if (activateBread.breadInBasket.Count > 0 && breadStackCurrentCount < breadStackMaxCount)
        {
            GameObject bread = activateBread.breadInBasket.Dequeue();
            activateBread.breadInPlayer.Push(bread);

            Rigidbody rb = bread.GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;

            bread.transform.SetParent(breadHolder, true);
            bread.transform.localPosition = new Vector3(0f, 0f, breadStackCurrentCount * -0.36f);
            bread.transform.localRotation = Quaternion.Euler(-90f, -90f, 90f);

            breadStackCurrentCount++;
            if(breadStackCurrentCount == breadStackMaxCount)
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

    public void DropBread()
    {
        if (breadStackCurrentCount <= 0 || displayBread.currentDisplayBreadCount >= displayBread.maxDisplayBreadCount)
        {
            return;
        }

        if(breadStackCurrentCount > 0 && breadStackCurrentCount <= breadStackMaxCount)
        {
            GameObject bread = activateBread.breadInPlayer.Pop();
            displayBread.displayBreadStack.Push(bread);

            Transform breadTr = displayBread.breadTr[displayBread.displayBreadStack.Count - 1];

            bread.transform.SetParent(breadTr, true);
            bread.transform.localScale = new Vector3(0.7f, 1f, 0.7f);
            bread.transform.localPosition = Vector3.zero;
            bread.transform.localRotation = Quaternion.identity;

            displayBread.currentDisplayBreadCount++;
            breadStackCurrentCount--;
            if (breadStackCurrentCount < breadStackMaxCount)
            {
                maxCountText.text = "";
            }
        }
    }

    public Vector3 GetMoveDirection() { return moveDirection; }
}
