using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using Random = UnityEngine.Random;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance
    {
        get; private set;
    }
    public static Action<bool> OnAddCard;
    public static event Action<bool> OnTurnStarted;

    public float turnTime;
    public TMP_Text timeTMP;

    float currentValue;
    bool countStart = false;
    bool timeWarning = true;

    public bool myTurn;
    public bool firstTurn;

    [Header("Properties")]
    public bool isLoading;

    WaitForSeconds delay05 = new WaitForSeconds(0.5f);


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
    }
    private void Update()
    {
        if (countStart && !isLoading)
            TurnTimer();
    }

    public void TurnSetup(int randomTurn)
    {
        bool isServer = NetworkRpcFunc.instance.isServer;

        if (randomTurn == 0)
            myTurn = isServer;
        else
            myTurn = !isServer;

        StartCoroutine(StartGameCo(myTurn));
    }

    public void TurnEnd()
    {
        firstTurn = false;
        myTurn = !myTurn;
        StartCoroutine(StartTurnCo(myTurn));
    }

    public IEnumerator StartGameCo(bool playerTurn)
    {
        myTurn = playerTurn;
        delay05 = new WaitForSeconds(0.05f);
        yield return delay05;

        firstTurn = true;

        StartCoroutine(StartTurnCo(playerTurn));

        isLoading = true;
    }

    public IEnumerator StartTurnCo(bool playerTurn)
    {
        isLoading = true;

        myTurn = playerTurn;
        currentValue = turnTime;
        OnTurnStarted?.Invoke(playerTurn);
        OnAddCard?.Invoke(playerTurn);

        if (myTurn)
        {
            GameManager.instance.Notification("내 턴! \n아이고 난!");
        }
        else
        {
            GameManager.instance.Notification("상대 턴!");
        }

        yield return delay05;

        countStart = true;
        isLoading = false;

        GameManager.instance.EndTurnBtnSetup(myTurn);
    }

    public void StartTurn()
    {
        GameManager.instance.localGamePlayerScript.CmdTurnEnd();
    }

    void TurnTimer()
    {
        if (currentValue > 0)
        {
            currentValue -= Time.deltaTime;
            timeTMP.text = "남은 시간 : " + ((int)currentValue).ToString() + "s";
            if ((int)currentValue == 10 && timeWarning)
            {
                timeWarning = false;
                GameManager.instance.Notification("10초\n남았습니다");
            }
        }
        else
        {
            timeTMP.text = "End";
            countStart = false;
            timeWarning = true;

            if (NetworkRpcFunc.instance.isServer)
                StartTurn();
        }
    }
}
