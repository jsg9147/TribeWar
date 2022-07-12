using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using Random = UnityEngine.Random;

public class SingleTurnManager : MonoBehaviour
{
    public static SingleTurnManager instance
    {
        get; private set;
    }
    public static Action<bool> OnAddCard;
    public static event Action<bool> OnTurnStarted;

    public bool isTutorial;

    public float turnTime;
    public TMP_Text timeTMP;

    float currentValue;
    bool countStart = false;
    bool timeWarning = true;

    public bool myTurn;
    public bool firstTurn;

    [Header("Properties")]
    public bool isLoading;

    [SerializeField][Header("턴 딜레이")] float turnDelay;
    WaitForSeconds delay;

    //private void Awake()
    //{
    //    instance = this;
    //}

    //private void Start()
    //{
    //}
    //private void Update()
    //{
    //    if (countStart && !isLoading && !isTutorial)
    //        TurnTimer();
    //}

    //void Init()
    //{
    //    delay = new WaitForSeconds(turnDelay);
    //}

    //public void TurnSetup(int randomTurn)
    //{
    //    myTurn = randomTurn == 0;

    //    StartCoroutine(StartGameCo(myTurn));
    //}

    //public void TurnEnd()
    //{
    //    firstTurn = false;
    //    myTurn = !myTurn;
    //    StartCoroutine(StartTurnCo(myTurn));
    //}

    //public IEnumerator StartGameCo(bool playerTurn)
    //{
    //    myTurn = playerTurn;
    //    yield return delay;

    //    firstTurn = true;

    //    StartCoroutine(StartTurnCo(playerTurn));

    //    isLoading = true;
    //}

    //public IEnumerator StartTurnCo(bool playerTurn)
    //{
    //    isLoading = true;

    //    myTurn = playerTurn;
    //    currentValue = turnTime;
    //    OnTurnStarted?.Invoke(playerTurn);
    //    OnAddCard?.Invoke(playerTurn);

    //    if (myTurn)
    //    {
    //        SingleManager.instance.Notification("턴 시작!");
    //    }
    //    else
    //    {
    //        SingleManager.instance.Notification("상대 턴!");
    //    }

    //    yield return delay;

    //    countStart = true;
    //    isLoading = false;
    //}

    //public void StartTurn()
    //{
    //    StartCoroutine(StartTurnCo(myTurn));
    //}

    //void TurnTimer()
    //{
    //    if (currentValue > 0)
    //    {
    //        currentValue -= Time.deltaTime;
    //        timeTMP.text = "남은 시간 : " + ((int)currentValue).ToString() + "s";
    //        if ((int)currentValue == 10 && timeWarning)
    //        {
    //            timeWarning = false;
    //            SingleManager.instance.Notification("10초\n남았습니다");
    //        }
    //    }
    //    else
    //    {
    //        timeTMP.text = "End";
    //        countStart = false;
    //        timeWarning = true;

    //        StartTurn();
    //    }
    //}
}
