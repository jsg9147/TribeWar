using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 나중엔 이거 파일로 저장되게 해야함
public class PlayLogControl : MonoBehaviour
{
    public static PlayLogControl instance;

    public PlayLog playLogPrefab;
    public PlayLog effectedLogPrefab;

    public GameObject logContent;

    [Header("옛날 변수")]
    public GameObject battleLogLayout;
    public GameObject singleLog;
    public GameObject effectLog;
    public GameObject effectedLog;
    public GameObject battleLog;
    public ScrollRect scrollRect;

    public Button bottomButton;

    // 단일 효과 or 몬스터카드 소환에 쓰이는 로그
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        bottomButton.onClick.AddListener(() =>
        {
            ScrollToBottom();
        });

        scrollRect.onValueChanged.AddListener(ScrollValueChange);
    }

    public void Log_Sorter(LogCategory logCategory, Entity fieldCard, Coordinate coord = null)
    {
        PlayLog playLog = (logCategory == LogCategory.Effected) ? Instantiate(effectedLogPrefab) : Instantiate(playLogPrefab);
        switch (logCategory)
        {
            case LogCategory.Summon:
                playLog.Monster_Log(fieldCard);
                break;
            case LogCategory.Move:
                playLog.Monster_Move_Log(fieldCard, coord);
                break;
            case LogCategory.Magic:
                playLog.Magic_Log(fieldCard.card);
                break;
            case LogCategory.Effected:
                playLog.Effected_Log(fieldCard);
                break;
            case LogCategory.Sacrifice:
                playLog.Monster_Log(fieldCard, "제물");
                break;
        }
        Add_Log(playLog);
    }

    public void Log_Sorter(LogCategory logCategory, Card card, bool isMine)
    {
        PlayLog playLog = Instantiate(playLogPrefab);
        switch (logCategory)
        {
            case LogCategory.Magic:
                playLog.Magic_Log(card);
                break;
            case LogCategory.Drop:
                playLog.Magic_Log(card, "패에서 버림");
                break;
        }
        Add_Log(playLog);
    }

    public void Log_Sorter(LogCategory logCategory, Entity card, int beforeBP, int afterBP)
    {
        PlayLog playLog = (logCategory == LogCategory.Attack) ? Instantiate(playLogPrefab) : Instantiate(effectedLogPrefab);
        switch (logCategory)
        {
            case LogCategory.Attack:
                playLog.Monster_Battle_Log(card, beforeBP, afterBP);
                break;
            case LogCategory.Defend:
                playLog.Monster_Battle_Log(card, beforeBP, afterBP, "피격");
                break;
        }

        Add_Log(playLog);
    }

    public void Log_Sorter(LogCategory logCategory, Entity card, Outpost outpost)
    {
        PlayLog attackerLog = Instantiate(playLogPrefab);
        PlayLog outpostLog = Instantiate(effectedLogPrefab);
        switch (logCategory)
        {
            case LogCategory.Outpost_Attack:
                attackerLog.Outpost_Attack_Log(card);
                outpostLog.Outpost_Damaged_Log(outpost, card);
                break;
        }
    }
    void Add_Log(PlayLog log)
    {
        log.transform.SetParent(battleLogLayout.transform, false);
        log.transform.localScale = new Vector3(1, 1, 1);
    }
    void ScrollToBottom()
    {
        scrollRect.verticalNormalizedPosition = 0f;
    }



    void ScrollValueChange(Vector2 value)
    {
        if (value.y != 0)
            bottomButton.transform.GetComponent<Transform>().localScale = Vector3.one;
        else
            bottomButton.transform.GetComponent<Transform>().localScale = Vector3.zero;
    }
}
