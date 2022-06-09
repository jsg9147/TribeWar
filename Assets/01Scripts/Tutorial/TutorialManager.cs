using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] Image tutorialBackground;
    [SerializeField] Image textArrow;

    [SerializeField][Header("화면 텍스트 딜레이")] float textTime;
    [SerializeField][Header("짧은 대기 시간")] float short_Time;

    [SerializeField] GameObject tutorialPanel;
    [SerializeField][Header("화면 중앙 설명 텍스트")] TMP_Text mainText;
    [SerializeField][Header("사용자 입력 대기 시간")] int timeToInput;
    [SerializeField][Header("텍스트 순서")] int textStep;


    [SerializeField][Header("게임 매니저")] SingleManager singleManager;


    [SerializeField] int tutorialStep;
    [SerializeField]
    float time_start, time_current, text_time_current;

    [SerializeField][Header("손패 가장 오른쪽")] Image rightCard_arrow;

    [SerializeField][Header("Step1 거점 설치")] GameObject step1_Panel;
    [SerializeField] Image step1_arrow1;
    [SerializeField] Image step1_arrow2;
    [SerializeField] GameObject textObj;

    [SerializeField][Header("Step2 몬스터 소환")] GameObject step2_Panel;

    [SerializeField] Image step2_arrow; // 손패
    [SerializeField] Image step2_arrow2; // 좌상단
    [SerializeField] Image step2_arrow3; // 소환위치

    [SerializeField][Header("턴 엔드 화살표")] Image step3_arrow;

    [SerializeField] GameObject step4_Panel;
    [SerializeField][Header("이동 텍스트")] TMP_Text step4_text;
    [SerializeField][Header("이동할 지점 표시")] Image step4_arrow;

    [SerializeField] GameObject step5_Panel;
    [SerializeField][Header("2차 소환 지점 표시")] Image step5_arrow;
    [SerializeField][Header("제물소환 지점 표시")] Image step5_arrow2;
    [SerializeField][Header("제물 고르라는 텍스트")] GameObject step5_text;

    [SerializeField][Header("전투력 올릴 카드")] Image step6_arrow;

    [SerializeField] EndTurnBtn endTurnBtn;

    [SerializeField][Header("카드 설명용 판넬")] GameObject card_Panel;
    [SerializeField][Header("카드 텍스트")] TMP_Text card_text;
    [SerializeField] Image level_arrow;
    [SerializeField] CardUI card_UI;
    [SerializeField] Image top_Arrow;
    [SerializeField] Image middle_Arrow;
    bool canNext;
    bool canNextText;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        NextStep();
        NextText();
    }

    void Init()
    {
        canNext = false;
        time_start = Time.time;
        time_current = 0;
        tutorialStep = 0;
        textStep = 0;
        card_Panel.SetActive(false);
        StartCoroutine(TutorialStart());
        FadeoutEffect(textArrow);
    }

    IEnumerator TutorialStart()
    {
        StartCoroutine(Step1());

        yield return new WaitWhile(() => tutorialStep < 1);

        StartCoroutine(Step2());
        yield return new WaitWhile(() => tutorialStep < 2);


        StartCoroutine(Step3());
        yield return new WaitWhile(() => tutorialStep < 3);
        Base_Panel_SetActive(false);

        //여기가 카드 설명
        StartCoroutine(Step4());
        yield return new WaitWhile(() => tutorialStep < 4);

        StartCoroutine(Step5());
        yield return new WaitWhile(() => tutorialStep < 5);

        StartCoroutine(Step6());
        yield return new WaitWhile(() => tutorialStep < 6);
        StartCoroutine(Step7());

    }

    IEnumerator Step1()
    {
        mainText.gameObject.SetActive(true);
        endTurnBtn.Setup(false);

        mainText.text = "지금부터 튜토리얼을 시작하겠습니다.";
        canNextText = true;
        yield return new WaitWhile(() => textStep < 1);

        mainText.text = "먼저 표시된곳을 클릭해 거점을 지어주세요!";
        yield return new WaitWhile(() => textStep < 2);

        SingleMapManager.instance.mapData[4, 2].ColorChange_Rock(true, Color.blue);
        SingleMapManager.instance.mapData[5, 2].ColorChange_Rock(true, Color.blue);


        foreach (var tile in SingleMapManager.instance.mapData)
        {
            if (tile == SingleMapManager.instance.mapData[4, 2] || tile == SingleMapManager.instance.mapData[5, 2])
            {
                tile.clickBlock = false;
            }
            else
            {
                tile.clickBlock = true;
            }
        }

        Step1_Arrow_Active(true);
        Base_Panel_SetActive(false);


        yield return new WaitWhile(() => !SingleManager.instance.isStarted);
        Step1_Arrow_Active(false);
        singleManager.LoadingComplited();
        Base_Panel_SetActive(true);

        canNext = true;
        textStep = 0;
        canNextText = false;

        mainText.text = "이제 기본적인 규칙에 대해 알아보겠습니다";

    }
    void Step1_Arrow_Active(bool isActive)
    {
        step1_arrow1.gameObject.SetActive(isActive);
        step1_arrow2.gameObject.SetActive(isActive);
        textObj.SetActive(isActive);

        if (isActive)
        {
            FadeoutEffect(step1_arrow1);
            FadeoutEffect(step1_arrow2);
        }
        else
        {
            FadeoutEffect(step1_arrow1, true);
            FadeoutEffect(step1_arrow2, true);
        }
    }

    void Base_Panel_SetActive(bool isActive)
    {
        mainText.gameObject.SetActive(isActive);

        float alpha = isActive ? 0.5f : 0;
        tutorialBackground.color = new Color(0, 0, 0, alpha);

        SingleManager.instance.clickBlock = isActive;
    }


    IEnumerator Step2()
    {
        SingleMapManager.instance.Tile_ClickBlock(false);
        step1_Panel.gameObject.SetActive(false);
        endTurnBtn.Setup(false);

        foreach (var hand in SingleCardManager.instance.playerCards)
        {
            hand.clickBlock = true;
        }

        step2_Panel.SetActive(true);
        SingleMapManager.instance.Tile_Color_Reset();

        canNextText = true;

        mainText.text = "턴 시작시 손패는 5장이 됩니다.";
        step2_arrow.gameObject.SetActive(true);
        FadeoutEffect(step2_arrow);
        yield return new WaitWhile(() => textStep < 1);
        canNextText = false;

        step2_arrow.gameObject.SetActive(false);
        FadeoutEffect(step2_arrow, true);

        canNextText = true;
        mainText.text = "현재 턴에 대한 기본적인 정보가 표시됩니다";
        step2_arrow2.gameObject.SetActive(true);
        FadeoutEffect(step2_arrow2);
        yield return new WaitWhile(() => textStep < 2);


        mainText.text = "한턴의 제한시간은 100초 입니다.";
        yield return new WaitWhile(() => textStep < 3);


        mainText.text = "첫 턴에는 1마리만 소환이 가능합니다. \n이후에는 2마리까지 소환 가능합니다";
        yield return new WaitWhile(() => textStep < 4);

        mainText.text = "한턴에 카드는 2장까지 움직일 수 있습니다.";
        yield return new WaitWhile(() => textStep < 5);

        step2_arrow2.gameObject.SetActive(false);
        FadeoutEffect(step2_arrow2, true);
        mainText.text = "몬스터 카드는 거점 주변에 소환 할수 있습니다.";
        SingleMapManager.instance.Can_Summon_Tile_Display(true);
        yield return new WaitWhile(() => textStep < 6);

        mainText.text = "표시된 곳에 몬스터를 소환 해보세요.!";
        yield return new WaitWhile(() => textStep < 7);
        canNextText = false;
        SingleMapManager.instance.mapData[4, 3].ColorChange_Rock(true, Color.blue);
        SingleMapManager.instance.mapData[4, 3].clickBlock = false;

        foreach (var hand in SingleCardManager.instance.playerCards)
        {
            hand.clickBlock = false;
        }

        Base_Panel_SetActive(false);

        step2_arrow3.gameObject.SetActive(true);
        FadeoutEffect(step2_arrow3);


        yield return new WaitWhile(() => SingleEntityManager.instance.summonCount > 0);

        SingleMapManager.instance.Can_Summon_Tile_Display(false);
        SingleMapManager.instance.Tile_ClickBlock(false);
        step2_arrow3.gameObject.SetActive(false);

        Base_Panel_SetActive(true);

        canNextText = true;
        mainText.text = "소환된 카드는 다음턴부터 행동이 가능합니다.";
        yield return new WaitWhile(() => textStep < 8);
        mainText.text = "이제 우측 하단의 턴 종료를 눌러 턴을 넘겨주세요.";

        step3_arrow.gameObject.SetActive(true);
        FadeoutEffect(step3_arrow);

        yield return new WaitWhile(() => textStep < 9);
        Base_Panel_SetActive(false);

        foreach (var hand in SingleCardManager.instance.playerCards)
        {
            hand.clickBlock = true;
        }

        endTurnBtn.Setup(true);

        yield return new WaitWhile(() => SingleTurnManager.instance.myTurn);
        Base_Panel_SetActive(true);
        step3_arrow.gameObject.SetActive(false);
        mainText.text = "이제 몬스터 이동을 해보겠습니다.";

        canNext = true;
        canNextText = false;
        textStep = 0;
    }


    IEnumerator Step3()
    {
        Base_Panel_SetActive(false);
        List<Coordinate> summonPos_List = SingleMapManager.instance.canAI_SummonPos;
        SingleMapManager.instance.Tile_Color_Reset();
        yield return new WaitForSeconds(textTime);

        Coordinate summonPos = new Coordinate(4, 5);
        SingleEntityManager.instance.Summon(false, "base-001", summonPos);

        yield return new WaitForSeconds(short_Time);

        SingleTurnManager.instance.TurnEnd();
        foreach (var hand in SingleCardManager.instance.playerCards)
        {
            hand.clickBlock = true;
        }
        endTurnBtn.Setup(false);
        yield return new WaitForSeconds(2f);

        Base_Panel_SetActive(true);

        foreach (var hand in SingleCardManager.instance.playerCards)
        {
            hand.clickBlock = true;
        }

        card_Panel.SetActive(true);
        card_UI.Setup(CardDatabase.instance.CardData("base-001"));

        canNextText = true;
        mainText.text = "";
        textArrow.gameObject.SetActive(false);
        card_text.text = "카드는 색칠된 방향으로 이동 가능합니다.";
        canNextText = true;
        yield return new WaitWhile(() => textStep < 1);


        card_text.text = "빨간색으로 색칠된 방향\n상하좌우로 이동합니다.";
        middle_Arrow.gameObject.SetActive(true);
        FadeoutEffect(middle_Arrow);
        yield return new WaitWhile(() => textStep < 2);

        FadeoutEffect(middle_Arrow, true);
        middle_Arrow.gameObject.SetActive(false);

        card_UI.Setup(CardDatabase.instance.CardData("base-003"));
        top_Arrow.gameObject.SetActive(true);
        FadeoutEffect(top_Arrow);
        card_text.text = "파란색으로 색칠된 방향\n대각선으로 이동합니다.";
        yield return new WaitWhile(() => textStep < 3);

        card_UI.Setup(CardDatabase.instance.CardData("base-020"));
        middle_Arrow.gameObject.SetActive(true);
        FadeoutEffect(top_Arrow, true);
        FadeoutEffect(top_Arrow);
        FadeoutEffect(middle_Arrow);

        card_text.text = "전부 색칠된 카드는\n8방향 이동이 가능합니다.";

        yield return new WaitWhile(() => textStep < 4);

        card_Panel.SetActive(false);
        mainText.text = "이제 캐릭터를 움직여보겠습니다. \n소환한 카드를 클릭해서 표시한 곳으로 움직이세요";
        textArrow.gameObject.SetActive(true);
        yield return new WaitWhile(() => textStep < 5);

        foreach (var tile in SingleMapManager.instance.mapData)
        {
            tile.clickBlock = true;
        }

        SingleMapManager.instance.mapData[4, 4].ColorChange_Rock(true, Color.blue);
        SingleMapManager.instance.mapData[4, 4].clickBlock = false;
        SingleMapManager.instance.mapData[4, 3].clickBlock = false;
        yield return new WaitForSeconds(textTime);
        mainText.text = "소환된 카드를 직접 클릭해서 \n방향을 확인할 수도 있습니다.";
        yield return new WaitWhile(() => textStep < 6);

        Base_Panel_SetActive(false);
        step4_Panel.SetActive(true);
        FadeoutEffect(step4_arrow);

        yield return new WaitWhile(() => SingleEntityManager.instance.canMoveCount > 1);

        Base_Panel_SetActive(true);
        step4_Panel.SetActive(false);
        SingleMapManager.instance.Tile_Color_Reset();
        foreach (var hand in SingleCardManager.instance.playerCards)
        {
            hand.clickBlock = false;
        }
        foreach (var tile in SingleMapManager.instance.mapData)
        {
            tile.clickBlock = true;
        }

        mainText.text = "잘 하셨습니다. 이제 제물소환에 대해 알아보겠습니다.";

        canNext = true;
        canNextText = false;
        textStep = 0;
    }

    IEnumerator Step4()
    {
        Base_Panel_SetActive(true);
        step4_Panel.SetActive(false);
        SingleMapManager.instance.Tile_Color_Reset();
        foreach (var hand in SingleCardManager.instance.playerCards)
        {
            hand.clickBlock = false;
        }
        foreach (var tile in SingleMapManager.instance.mapData)
        {
            tile.clickBlock = true;
        }

        canNextText = true;

        card_Panel.SetActive(true);
        middle_Arrow.gameObject.SetActive(false);
        top_Arrow.gameObject.SetActive(false);
        level_arrow.gameObject.SetActive(true);
        FadeoutEffect(level_arrow);
        mainText.text = "";
        card_text.text = "카드의 좌측 하단에 있는 숫자는 카드 레벨입니다.";
        yield return new WaitWhile(() => textStep < 1);

        card_text.text = "상급 카드는 레벨 - 1 개의 제물이 필요합니다.";
        yield return new WaitWhile(() => textStep < 2);
        card_Panel.SetActive(false);

        mainText.text = "우선 1레벨 카드를 하나 소환 합니다.";
        yield return new WaitWhile(() => textStep < 3);
        canNextText = false;

        Base_Panel_SetActive(false);

        foreach (var hand in SingleCardManager.instance.playerCards)
        {
            if (hand.card.cost > 0)
                hand.clickBlock = true;
        }
        SingleMapManager.instance.mapData[4, 3].clickBlock = false;
        SingleMapManager.instance.mapData[4, 3].ColorChange_Rock(true, Color.blue);
        step5_arrow.gameObject.SetActive(true);
        FadeoutEffect(step5_arrow);

        yield return new WaitWhile(() => SingleEntityManager.instance.summonCount > 1);

        SingleMapManager.instance.Tile_Color_Reset();

        Base_Panel_SetActive(true);
        mainText.text = "이제 2레벨 카드를 소환 하세요.";
        canNextText = true;
        yield return new WaitWhile(() => textStep < 4);
        canNextText = false;

        step5_arrow.gameObject.SetActive(false);
        step5_arrow2.gameObject.SetActive(true);
        rightCard_arrow.gameObject.SetActive(true);
        FadeoutEffect(rightCard_arrow);
        FadeoutEffect(step5_arrow2);

        Base_Panel_SetActive(false);

        foreach (var hand in SingleCardManager.instance.playerCards)
        {
            if (hand.card.cost < 1)
            {
                hand.clickBlock = true;
            }
            else
            {
                hand.clickBlock = false;
            }
        }

        SingleMapManager.instance.mapData[4, 3].clickBlock = true;
        SingleMapManager.instance.mapData[5, 3].clickBlock = false;
        SingleMapManager.instance.mapData[5, 3].ColorChange_Rock(true, Color.blue);

        SingleEntityManager.instance.playerEntities[0].clickBlock = true;
        yield return new WaitWhile(() => SingleEntityManager.instance.tributeSummon == false);
        rightCard_arrow.gameObject.SetActive(false);
        step5_arrow2.gameObject.SetActive(false);
        yield return new WaitWhile(() => SingleEntityManager.instance.tutorial_Summon == false);

        step5_arrow2.gameObject.SetActive(false);
        step2_arrow3.gameObject.SetActive(true);

        step5_text.SetActive(true);
        yield return new WaitWhile(() => SingleEntityManager.instance.summonCount > 0);
        step5_text.SetActive(false);
        step2_arrow3.gameObject.SetActive(false);
        Base_Panel_SetActive(true);

        SingleMapManager.instance.Tile_Color_Reset();
        canNextText = true;
        mainText.text = "카드의 이동 거리는 레벨과 같습니다";
        yield return new WaitWhile(() => textStep < 5);

        mainText.text = "1레벨은 1칸, 2레벨은 2칸씩 움직입니다.";
        yield return new WaitWhile(() => textStep < 6);

        mainText.text = "턴 종료를 눌러주세요.";
        SingleEntityManager.instance.playerEntities[0].clickBlock = false;
        yield return new WaitWhile(() => textStep < 7);
        canNextText = false;

        step3_arrow.gameObject.SetActive(true);
        endTurnBtn.Setup(true);
        Base_Panel_SetActive(false);

        yield return new WaitWhile(() => SingleTurnManager.instance.myTurn);
        endTurnBtn.Setup(false);
        step3_arrow.gameObject.SetActive(false);
        SingleTurnManager.instance.TurnEnd();
        foreach (var hand in SingleCardManager.instance.playerCards)
        {
            if (hand.card.cardType.card_category != CardCategory.Magic)
                hand.clickBlock = true;
            else
                hand.clickBlock = false;
        }
        Base_Panel_SetActive(true);

        SingleMapManager.instance.Tile_Color_Reset();

        mainText.text = "마법카드 사용을 배워보도록 하겠습니다.";

        canNext = true;
        canNextText = false;
        textStep = 0;
    }

    IEnumerator Step5()
    {
        canNextText = true;
        mainText.text = "돌진 명령은 1턴간 전투력을 올려주는 카드입니다.";
        yield return new WaitWhile(() => textStep < 1);

        mainText.text = "가장 우측에 있는 카드를 사용해주세요.";
        yield return new WaitWhile(() => textStep < 2);
        canNextText = false;

        rightCard_arrow.gameObject.SetActive(true);
        foreach (var tile in SingleMapManager.instance.mapData)
        {
            tile.clickBlock = false;
        }

        Base_Panel_SetActive(false);

        yield return new WaitWhile(() => SingleCardManager.instance.playerUsedCardCount < 1);
        rightCard_arrow.gameObject.SetActive(false);

        Base_Panel_SetActive(true);

        foreach (var entity in SingleEntityManager.instance.playerEntities)
        {
            if (entity != SingleEntityManager.instance.playerEntities[0])
                entity.clickBlock = true;
        }

        canNextText = true;
        mainText.text = "필드의 표시된 카드를 선택해주세요.";
        yield return new WaitWhile(() => textStep < 3);
        canNextText = false;

        step6_arrow.gameObject.SetActive(true);
        FadeoutEffect(step6_arrow);
        Base_Panel_SetActive(false);

        yield return new WaitWhile(() => SingleEntityManager.instance.effect_Count < 1);
        FadeoutEffect(step6_arrow, true);
        step6_arrow.gameObject.SetActive(false);
        Base_Panel_SetActive(true);
        canNextText = true;
        mainText.text = "잘 하셨습니다.";
        yield return new WaitWhile(() => textStep < 4);
        canNextText = false;
        mainText.text = "이제 전투를 배워보도록 하겠습니다.";

        canNext = true;
        canNextText = false;
        textStep = 0;
    }

    IEnumerator Step6()
    {
        canNext = false;
        canNextText = true;
        mainText.text = "이 게임은 기본적으로 전투력끼리 대결을 합니다.";
        yield return new WaitWhile(() => textStep < 1);

        mainText.text = "앞에 있는 카드로 상대 몬스터를 공격 해보세요.";
        yield return new WaitWhile(() => textStep < 2);


        step6_arrow.gameObject.SetActive(true);
        Base_Panel_SetActive(false);

        foreach (var hand in SingleCardManager.instance.playerCards)
        {
            hand.clickBlock = true;
        }

        foreach (var tile in SingleMapManager.instance.mapData)
        {
            tile.clickBlock = true;

            if (tile == SingleMapManager.instance.mapData[4, 5])
                tile.clickBlock = false;
        }

        yield return new WaitWhile(() => SingleEntityManager.instance.canMoveCount > 1);
        step6_arrow.gameObject.SetActive(false);
        Base_Panel_SetActive(true);
        mainText.text = "공격을 하게되면 서로의 전투력을 차감해서 0 이된쪽은 파괴가 됩니다.";
        yield return new WaitWhile(() => textStep < 3);

        mainText.text = "카드 효과로 전투력이 상승한 상태라면\n효과가 끝나 전투력이 0 이 된다면 파괴 됩니다.";
        yield return new WaitWhile(() => textStep < 4);

        mainText.text = "턴 종료를 눌러주세요.";
        yield return new WaitWhile(() => textStep < 5);
        endTurnBtn.Setup(true);
        Base_Panel_SetActive(false);

        yield return new WaitWhile(() => SingleTurnManager.instance.myTurn);
        endTurnBtn.Setup(false);
        Coordinate summonPos = new Coordinate(4, 5);
        SingleEntityManager.instance.Summon(false, "base-004", summonPos);
        SingleTurnManager.instance.TurnEnd();

        Base_Panel_SetActive(true);
        mainText.text = "카드 타입에 대해 알아보겠습니다.";

        canNext = true;
        canNextText = false;
        textStep = 0;
    }

    IEnumerator Step7()
    {
        canNextText = true;
        yield return new WaitWhile(() => textStep < 1);
        mainText.text = "카드는 일반, 원거리, 이동 타입이 있습니다.";
        yield return new WaitWhile(() => textStep < 2);
        mainText.text = "일반은 앞서 사용한 카드와 같습니다.";
        yield return new WaitWhile(() => textStep < 3);
        mainText.text = "다른 카드들은 전투시 반격데미지를 2배 받습니다.";
        yield return new WaitWhile(() => textStep < 4);
        mainText.text = "원거리는 이동은 한칸씩만 가능하지만\n공격시 반격데미지를 받지 않습니다.";
        yield return new WaitWhile(() => textStep < 5);
        mainText.text = "이동은 이동거리가 1칸 더 길어집니다.";
        yield return new WaitWhile(() => textStep < 6);
        mainText.text = "이제 원거리 몬스터를 사용해보겠습니다.";
        yield return new WaitWhile(() => textStep < 7);
        mainText.text = "오른쪽의 원거리 몬스터를 소환하세요.";
        yield return new WaitWhile(() => textStep < 8);

        canNextText = false;

        Base_Panel_SetActive(false);
        rightCard_arrow.gameObject.SetActive(true);
        step5_arrow.gameObject.SetActive(true);
        foreach (var tile in SingleMapManager.instance.mapData)
        {
            tile.clickBlock = true;
        }

        SingleMapManager.instance.mapData[4, 3].clickBlock = false;
        SingleMapManager.instance.mapData[4, 3].ColorChange_Rock(true, Color.blue);

        yield return new WaitWhile(() => SingleEntityManager.instance.summonCount > 1);
        rightCard_arrow.gameObject.SetActive(false);
        step5_arrow.gameObject.SetActive(false);
        Base_Panel_SetActive(true);

        canNextText = true;
        mainText.text = "턴을 종료해주세요.";
        yield return new WaitWhile(() => textStep < 9);
        canNextText = false;

        endTurnBtn.Setup(true);
        Base_Panel_SetActive(false);
        yield return new WaitWhile(() => SingleTurnManager.instance.myTurn);
        endTurnBtn.Setup(false);

        int entity_ID = SingleEntityManager.instance.opponentEntities[0].id;
        yield return new WaitForSeconds(1f);

        SingleEntityManager.instance.CardMove(entity_ID, false, new Coordinate(4, 4), true);
        SingleTurnManager.instance.TurnEnd();

        Base_Panel_SetActive(true);
        canNextText = true;
        mainText.text = "앞의 카드를 공격해주세요.";
        yield return new WaitWhile(() => textStep < 10);
        canNextText = false;

        Base_Panel_SetActive(false);
        foreach (var hand in SingleCardManager.instance.playerCards)
        {
            hand.clickBlock = true;
        }

        foreach (var entity in SingleEntityManager.instance.playerEntities)
        {
            if (entity.card.cardType.attack_type != AttackType.shooter)
            {
                entity.clickBlock = true;
            }
        }

        foreach (var tile in SingleMapManager.instance.mapData)
        {
            if (tile != SingleMapManager.instance.mapData[4, 4])
                tile.clickBlock = true;
            else
                tile.clickBlock = false;
        }

        yield return new WaitWhile(() => SingleEntityManager.instance.canMoveCount > 1);

        Base_Panel_SetActive(true);
        canNextText = true;
        mainText.text = "지금처럼 원거리 카드는 공격시 반격 데미지를 받지 않습니다.";
        yield return new WaitWhile(() => textStep < 11);

        mainText.text = "그러나 데미지는 2배로 받고 이동거리는 1칸으로 고정되어 있습니다.";
        yield return new WaitWhile(() => textStep < 12);

        mainText.text = "턴종료를 눌러주세요.";
        yield return new WaitWhile(() => textStep < 13);
        canNextText = false;
        Base_Panel_SetActive(false);

        endTurnBtn.Setup(true);
        yield return new WaitWhile(() => SingleTurnManager.instance.myTurn);

        int defender_Id, attacker_Id;

        defender_Id = SingleEntityManager.instance.playerEntities[1].id;
        attacker_Id = SingleEntityManager.instance.opponentEntities[0].id;

        SingleEntityManager.instance.Attack(attacker_Id, defender_Id, false);

        yield return new WaitForSeconds(2f);
        Base_Panel_SetActive(true);
        canNextText = true;
        mainText.text = "지금처럼 원거리는\n공격받을때 2배의 데미지를 받습니다.";
        yield return new WaitWhile(() => textStep < 14);

        mainText.text = "이제 적의 거점을 파괴하세요.";
        yield return new WaitWhile(() => textStep < 15);
        canNextText = false;
        SingleTurnManager.instance.TurnEnd();

        endTurnBtn.Setup(true);
        endTurnBtn.AddTrunAction();
        Base_Panel_SetActive(false);

        foreach (var hand in SingleCardManager.instance.playerCards)
        {
            hand.clickBlock = false;
        }

        foreach (var entity in SingleEntityManager.instance.playerEntities)
        {
            if (entity.card.cardType.attack_type != AttackType.shooter)
                entity.clickBlock = false;
        }

        foreach (var tile in SingleMapManager.instance.mapData)
        {
            if (tile != SingleMapManager.instance.mapData[4, 4])
                tile.clickBlock = false;
        }

        canNext = true;
        canNextText = false;
        textStep = 0;



        endTurnBtn.GetComponent<Button>().onClick.AddListener(() => TurnEnd());
    }


    void FadeoutEffect(Image fadeImg, bool doKill = false)
    {
        if (doKill)
        {
            fadeImg.DOKill();
            fadeImg.color = new Color(255, 255, 255, 1);
        }
        else
            fadeImg.DOFade(0, 1).SetEase(Ease.InSine).SetLoops(-1, LoopType.Restart);

    }

    void NextStep()
    {
        if (canNext == false)
            return;

        if (Input.GetKeyDown(KeyCode.Mouse0) && time_current > timeToInput)
        {
            tutorialStep++;
            time_start = Time.time;
            canNext = false;
        }
        time_current = Time.time - time_start;
    }

    void NextText()
    {
        if (canNextText == false)
            return;

        if (Input.GetKeyDown(KeyCode.Mouse0) && text_time_current > textTime)
        {
            textStep++;
            time_start = Time.time;
        }
        text_time_current = Time.time - time_start;
    }

    IEnumerator AI_TurnEnd()
    {
        yield return new WaitForSeconds(1f);
        {
            SingleTurnManager.instance.TurnEnd();
        }
    }

    void TurnEnd()
    {
        StartCoroutine(AI_TurnEnd());
    }
}
