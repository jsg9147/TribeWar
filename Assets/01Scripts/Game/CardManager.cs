using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Steamworks;
using Mirror;

using Random = UnityEngine.Random;
using System.Linq;

public class CardManager : MonoBehaviour
{
    public static CardManager instance
    {
        get; private set;
    }


    public Canvas canvas;
    public EffectManager effectManager;

    [Header("Deck remain card List")]
    public CardUI deckCard;
    public Transform deck_Content;

    [Header("Object")]
    [SerializeField] bool TestGame;
    [SerializeField] CameraEffect cameraEffect;
    [SerializeField] GameObject handPrefab;
    [SerializeField] GameObject playerCardParent;
    [SerializeField] GameObject opponentCardParent;

    [SerializeField] CardUI EnlargeEffectCard;

    [Header("Card Spawn Position")]
    [SerializeField] Transform playerCardSpawnPoint;
    [SerializeField] Transform opponentCardSpawnPoint;

    [Header("Card Alignment")]
    [SerializeField] Transform myCardLeft;
    [SerializeField] Transform myCardRight;
    [SerializeField] Transform otherCardLeft;
    [SerializeField] Transform otherCardRight;
    [SerializeField] EcardState ecardState;

    [SerializeField] TMPro.TMP_Text playerDeckCounting;
    [SerializeField] TMPro.TMP_Text opponentDeckCounting;

    List<Hand> playerCards;
    List<Hand> opponentCards;

    public int maxHandCard;

    List<Card> itemBuffer;
    List<CardUI> viewList;

    Hand selectCard = null;
    public bool selectState => selectCard != null;

    bool isMyCardDrag;

    int playerSummonCount, playerUsedCardCount = -1;
    int myCardIndex;

    Hand tempTributeCard;

    void Awake() => instance = this;

    private void Start()
    {
        Init();
    }


    private void Update()
    {
        if (isMyCardDrag)
            CardDrag();

        SetEcardState();
    }
    private void OnDestroy()
    {
        TurnManager.OnAddCard -= AddCard;
        TurnManager.OnTurnStarted -= TurnEndSetup;
    }

    void Init()
    {
        playerCards = new List<Hand>();
        opponentCards = new List<Hand>();

        TurnManager.OnAddCard += AddCard;
        TurnManager.OnTurnStarted += TurnEndSetup;
        Playing_Deck_Init();

    }

    void Playing_Deck_Init()
    {
        SetupItemBuffer();
        viewList = new List<CardUI>();

        for (int i = 0; i < itemBuffer.Count; i++)
        {
            CardUI cardUI = Instantiate(deckCard, deck_Content);
            viewList.Add(cardUI);
        }
        Deck_Item_Update();
        deck_Content.transform.GetComponent<FlexibleGrid>().SetFlexibleGrid();
    }
    void Deck_Item_Update()
    {
        for (int i = 0; i < viewList.Count; i++)
        {
            if (i < itemBuffer.Count)
            {
                viewList[i].Setup(itemBuffer[i]);
                viewList[i].transform.gameObject.SetActive(true);
            }
            else
            {
                viewList[i].transform.gameObject.SetActive(false);
            }
        }
        viewList.OrderBy(x => x.card.id).ThenBy(x => x.card.name);
    }

    public Card PopItem(bool isMine)
    {
        if (isMine)
        {
            if (itemBuffer.Count == 0)
            {
                if (TestGame)
                {
                    SetupItemBuffer();
                }
            }
            Card card = itemBuffer[0];
            itemBuffer.RemoveAt(0);
            GameManager.instance.localGamePlayerScript.CmdSetDeckCounting(itemBuffer.Count);
            Deck_Item_Update();
            return card;
        }

        return null;
    }

    public void Deck_Counting_Update(bool isMine, int deckCount)
    {
        if (isMine)
            playerDeckCounting.text = itemBuffer.Count.ToString();
        else
            opponentDeckCounting.text = deckCount.ToString();
    }

    public void MouseEventInit()
    {
        isMyCardDrag = false;
        selectCard?.MoveTransform(selectCard.originPRS, false);
    }

    void SetupItemBuffer()
    {
        Deck selected_Deck = new Deck();

        Dictionary<string, int> deckItem = new Dictionary<string, int>();

        if (WebMain.instance != null)
        {
            selected_Deck = WebMain.instance.web.selected_Deck;
            deckItem = selected_Deck.cardCount;
        }

        itemBuffer = new List<Card>();

        foreach (var card_id in deckItem.Keys)
        {
            Card card = CardDatabase.instance.CardData(card_id);

            for (int i = 0; i < deckItem[card_id]; i++)
            {
                itemBuffer.Add(card);
            }
        }

        for (int i = 0; i < itemBuffer.Count; i++)
        {
            int rand = Random.Range(i, itemBuffer.Count);
            Card temp = itemBuffer[i];
            itemBuffer[i] = itemBuffer[rand];
            itemBuffer[rand] = temp;
        }

        playerDeckCounting.text = itemBuffer.Count.ToString();
    }

    // 지금 방식은 드로우 모션이 구리니까 변경이 필요 jsg
    void AddCard(bool isMine)
    {
        GameObject parent = isMine ? playerCardParent : opponentCardParent;
        var cardSpawnPoint = isMine ? playerCardSpawnPoint : opponentCardSpawnPoint;

        if (playerUsedCardCount == 0 && isMine)
        {
            GameManager.instance.localGamePlayerScript.CmdReloadHandCard(NetworkRpcFunc.instance.isServer);
        }
        else
        {
            for (int i = isMine ? playerCards.Count : opponentCards.Count; i < maxHandCard; i++)
            {
                if (itemBuffer.Count == 0 && isMine)
                {
                    if (TestGame == false)
                    {
                        GameManager.instance.GameResult(false);
                        break;
                    }
                }
                var cardObject = Instantiate(handPrefab, cardSpawnPoint.position, Utils.QI);
                cardObject.SetActive(true);
                cardObject.transform.SetParent(parent.transform);
                cardObject.transform.localScale = Vector3.one;
                var card = cardObject.GetComponent<Hand>();
                card.Setup(PopItem(isMine), isMine);
                cardObject.name = card.card.name;
                (isMine ? playerCards : opponentCards).Add(card);
            }
            CardAlignment(isMine, 0.8f);
        }

        if (TurnManager.instance.myTurn)
        {
            playerUsedCardCount = 0;
        }

    }

    public void StartCardDealing()
    {
        StartCoroutine(ReloadHandCard_Coroutine(true));
        StartCoroutine(ReloadHandCard_Coroutine(false));
    }

    public void ReloadCard(bool server)
    {
        bool isMine = NetworkRpcFunc.instance.isServer == server;
        StartCoroutine(ReloadHandCard_Coroutine(isMine));
    }

    public IEnumerator ReloadHandCard_Coroutine(bool isMine)
    {
        var targetCards = isMine ? playerCards : opponentCards;
        var cardSpawnPoint = isMine ? playerCardSpawnPoint : opponentCardSpawnPoint;
        GameObject cardObjectParent = isMine ? playerCardParent : opponentCardParent;
        foreach (Hand handCard in targetCards)
        {
            handCard.transform.DOKill(true);
            if (isMine)
            {
                Card reloadCard = new Card(handCard.card);
                itemBuffer.Add(reloadCard);
            }
            handCard.transform.DOMove(cardSpawnPoint.transform.position, 0.2f);
            yield return new WaitForSeconds(0.2f);
            handCard.transform.DOKill(true);
            Destroy(handCard.gameObject);
        }
        targetCards.Clear();
        for (int i = targetCards.Count; i < maxHandCard; i++)
        {
            if (itemBuffer.Count == 0)
                break;

            var cardObject = Instantiate(handPrefab, cardSpawnPoint.position, Utils.QI);
            cardObject.transform.SetParent(cardObjectParent.transform);
            cardObject.transform.localScale = Vector3.one;
            var card = cardObject.GetComponent<Hand>();
            card.Setup(PopItem(isMine), isMine);
            cardObject.name = card.card.name;
            targetCards.Add(card);
            card.gameObject.SetActive(true);
        }

        CardAlignment(isMine, 0.8f);
    }

    void TestAligment(float alignmentTIme = 0f)
    {
        var originCardPRSs = RoundAlignment(myCardLeft, myCardRight, playerCards.Count, 0.5f, Vector3.one);
        int targetIndex = playerCards.FindIndex(x => x == selectCard);

        if (targetIndex == -1)
            return;

        var targetHand = playerCards[targetIndex];
        targetHand.originPRS = originCardPRSs[targetIndex];
        targetHand.MoveTransform(targetHand.originPRS, true, alignmentTIme);
        foreach (var hand in playerCards)
        {
            if(hand != null)
                hand.Can_Use_Effect(EntityManager.instance.canUseCard(true, hand.card));
        }
    }

    void CardAlignment(bool isMine, float alignmentTIme = 0f)
    {
        List<PRS> originCardPRSs = new List<PRS>();
        if (isMine)
        {
            originCardPRSs = RoundAlignment(myCardLeft, myCardRight, playerCards.Count, 0.5f, Vector3.one);
        }
        else
        {
            originCardPRSs = RoundAlignment(otherCardLeft, otherCardRight, opponentCards.Count, -0.5f, Vector3.one);
        }

        var targetCards = isMine ? playerCards : opponentCards;
        for (int i = 0; i < targetCards.Count; i++)
        {
            var targetCard = targetCards[i];
            targetCard.originPRS = originCardPRSs[i];
            targetCard.MoveTransform(targetCard.originPRS, true, alignmentTIme);
        }

        foreach (var hand in playerCards)
        {
            hand.Can_Use_Effect(EntityManager.instance.canUseCard(true, hand.card));
        }
    }

    // jsg 카드 정렬함수, 문제 생길수 있으니 생기면 찾아봐야함
    List<PRS> RoundAlignment(Transform leftTr, Transform rightTr, int objCount, float height, Vector3 scale)
    {
        float[] objLerps = new float[objCount];
        List<PRS> result = new List<PRS>(objCount);

        switch (objCount)
        {
            case 1: objLerps = new float[] { 0.5f }; break;
            case 2: objLerps = new float[] { 0.27f, 0.73f }; break;
            case 3: objLerps = new float[] { 0.1f, 0.5f, 0.9f }; break;
            default:
                float interval = 1f / (objCount - 1);
                for (int i = 0; i < objCount; i++)
                    objLerps[i] = interval * i;
                break;
        }

        for (int i = 0; i < objCount; i++)
        {
            var targetPos = Vector3.Lerp(leftTr.position, rightTr.position, objLerps[i]);
            var targetRot = Quaternion.identity;

            result.Add(new PRS(targetPos, targetRot, scale));
        }

        return result;
    }

    private void CardDrag()
    {
        if (selectCard != null)
        {
            selectCard.MoveTransform(new PRS(new Vector3(Utils.MousePos.x, Utils.MousePos.y + 15, Utils.MousePos.z), Utils.QI, selectCard.originPRS.scale), false);
            selectCard.modulateAlpha(0.5f);
        }
    }

    public void TryPutCard(bool server, string card_id, Vector3 selectPos)
    {
        bool isMine = NetworkRpcFunc.instance.isServer == server;
        if (card_id == null || EntityManager.instance.SelectMonsterMode) { return; }

        List<Hand> targetList = isMine ? playerCards : opponentCards;
        Hand handCard = targetList.Find(x => x.card.id == card_id);
        Card tryCard = CardDatabase.instance.CardData(card_id);
        Coordinate selectCoord = new Coordinate(selectPos);

        tempTributeCard = handCard;

        //제물 효과시 카드 제거 해주는 변수
        if (isMine)
        {
            myCardIndex = targetList.IndexOf(handCard);
        }

        if (tryCard.cardType.card_category == CardCategory.Monster)
        {
            EntityManager.instance.Summon(isMine , card_id, selectCoord);
        }
        else
        {
            if (effectManager.EffectTrigger(isMine, card_id))
            {
                if (isMine)
                {
                    targetList.Remove(handCard);
                    handCard.transform.DOKill();
                    Destroy(handCard.gameObject);
                    selectCard = null;
                    playerUsedCardCount++;
                }
                else
                {
                    RemoveTargetCards(isMine);
                }

                if (tryCard.cardType.card_category != CardCategory.Monster)
                    EffectCard_Animation(tryCard);

                CardAlignment(isMine);
            }
            else
            {
                CardAlignment(isMine);
            }
        }

        EnlargeCardManager.instance.Setup(tryCard, true);
    }

    void EffectCard_Animation(Card triggerCard)
    {
        CardUI triggerCardUI = Instantiate(EnlargeEffectCard, canvas.transform);
        triggerCardUI.Setup(triggerCard);

        Sequence sequence = DOTween.Sequence()
        .Append(triggerCardUI.transform.DOMoveX(-120f, 0.8f))
        .AppendCallback(() =>
        {

        }).OnComplete(() => Destroy(triggerCardUI));
        triggerCardUI.transform.DOScale(Vector3.zero, 0.8f);
    }

    // EntityManager의 Summon 에서 제물소환시 카드를 숨겼다 하면 다시 보이게 하려고 만듬 함수
    public void TributeSummonSet(bool tributeSummon)
    {
        if (tempTributeCard == null)
            return;

        tempTributeCard.gameObject.SetActive(tributeSummon);

        if (tributeSummon)
        {
            tempTributeCard = null;
        }

        playerUsedCardCount++;
    }

    void TurnEndSetup(bool myTurn)
    {
        if (myTurn == false && tempTributeCard != null)
            tempTributeCard.gameObject.SetActive(true);
    }

    // 상대가 카드 발동시 상대 패에서 나가는 애니메이션 동작하게 하는거
    public void Remove_OhterPlayer_HandCard()
    {
        Hand card = opponentCards[Random.Range(0, opponentCards.Count)];
        card.transform.DOKill();

        opponentCards.Remove(card);
        Destroy(card.gameObject);

        CardAlignment(false);
    }

    public void SetMostFrontOrderInit(bool server)
    {
        bool isMine = server == NetworkRpcFunc.instance.isServer;
        //var targetList = isMine ? playerCards : opponentCards;
        //targetList.ForEach(x => x.GetComponent<Order>().SetMostFrontOrder(false));
        CardAlignment(isMine);
    }

    public void RemoveTargetCards(bool isMine)
    {
        if (isMine)
        {
            Hand card = playerCards[myCardIndex];
            var targetCards = playerCards;
            targetCards.Remove(card);
            card.transform.DOKill();
            Destroy(card.gameObject);
            if (isMine)
            {
                selectCard = null;
                playerSummonCount++;
                playerUsedCardCount++;
            }
            CardAlignment(isMine);

            myCardIndex = 0;
        }
        else
        {
            Remove_OhterPlayer_HandCard();
        }
    }

    public void OnTurnStarted(bool myTurn)
    {
        if (myTurn)
        {
            playerSummonCount = 0;
        }
    }




    #region MyCard

    public void CardMouseOver(Hand card)
    {
        if (ecardState == EcardState.Nothing)
            return;

        if (isMyCardDrag)
            return;

        selectCard = card;
        EnlargeCard(true, card);
    }

    public void CardMouseExit(Hand card)
    {
        EnlargeCard(false, card);

        if (isMyCardDrag)
            return;

        selectCard = null;
    }

    public void CardMouseDown(Hand card)
    {
        if (ecardState != EcardState.CanMouseDrag)
            return;

        isMyCardDrag = true;
    }

    public void CardMouseUp(Hand handCard)
    {
        isMyCardDrag = false;

        if (ecardState != EcardState.CanMouseDrag)
            return;

        CardAlignment(true);

        if (selectCard != null && EntityManager.instance.selectTile != null)
        {
            GameManager.instance.localGamePlayerScript.CmdTryPutCard(NetworkRpcFunc.instance.isServer, handCard.card.id, EntityManager.instance.selectTile.coordinate.vector3Pos);
        }
    }

    void EnlargeCard(bool isEnlarge, Hand card)
    {
        // card.GetComponent<Order>().SetMostFrontOrder(isEnlarge);
        if (isEnlarge)
        {
            Vector3 enlargePos = new Vector3(card.originPRS.pos.x, card.originPRS.pos.y + 50, 0f);
            card.MoveTransform(new PRS(enlargePos, Utils.QI, Vector3.one * 1.5f), false);
            card.transform.SetAsLastSibling();
            card.modulateAlpha(0.5f);
        }
        else
        {
            TestAligment();
            card.modulateAlpha(1f);
            //CardAlignment(true);
        }
    }

    private void SetEcardState()
    {
        if (TurnManager.instance == null)
        {
            return;
        }

        if (TurnManager.instance.isLoading)
            ecardState = EcardState.Nothing;

        else if (TurnManager.instance.myTurn == false) // 기존 코드에서 내턴 카드 제한을 풀었음
            ecardState = EcardState.CanMouseOver;

        else if (TurnManager.instance.myTurn)
            ecardState = EcardState.CanMouseDrag;
    }

    #endregion
}
