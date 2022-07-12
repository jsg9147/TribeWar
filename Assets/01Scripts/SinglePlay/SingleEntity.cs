using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class SingleEntity : MonoBehaviour
{
    private const int PERMANENT = 0;

    [SerializeField] SpriteRenderer feildCardFrame;
    [SerializeField] SpriteRenderer cardSprite;
    [SerializeField] TMP_Text BattlePower_TMP;
    [SerializeField] SpriteRenderer arrow;

    public Sprite bishopFrame;
    public Sprite rookFrame;
    public Sprite queenFrame;

    public Card card;

    string card_id;
    public SingleTile bottomTile
    {
        get
        {
            return SingleMapManager.instance.mapData[coordinate.x, coordinate.y];
        }
    }

    public bool isMine;
    public bool isDie;

    public bool attackable;

    //public Vector3 transformPos
    //{
    //    get
    //    {
    //        return transform.position;
    //    }
    //    set
    //    {
    //        MoveTransform(value, true, 0.5f);
    //    }
    //}

    public int id;
    public Coordinate coordinate;

    public int liveCount;

    public int moveCount;
    public bool canTribute;
    public bool clickBlock;

    int battlePower;

    Color originColor;

    //public void Setup(Card _card, bool _isMine)
    //{
    //    this.isMine = _isMine;
    //    this.card = _card;
    //    coordinate = new Coordinate();

    //    if (card.cardType.card_category == CardCategory.Monster)
    //    {
    //        battlePower = card.GetBaseStat("bp");
    //    }

    //    if (isMine)
    //    {
    //        originColor = new Color(0, 146, 255);
    //    }
    //    else
    //    {
    //        originColor = Color.red;
    //    }

    //    cardSprite.sprite = this.card.sprite;
    //    BattlePower_TMP.text = battlePower.ToString();
    //    moveCount = 0;
    //    liveCount = 0;
    //    canTribute = true;

    //    if (isMine == false)
    //    {
    //        cardSprite.transform.rotation = Quaternion.Euler(0, 0, 180);
    //    }
    //    feildCardFrame.color = originColor;
    //    clickBlock = false;

    //    card_id = card.id;
    //}

    //public void FrameColorRefresh() => feildCardFrame.color = originColor;

    //#region Mouse activate

    //private void OnMouseOver()
    //{
    //    if (clickBlock)
    //        return;
    //    if (SingleManager.instance?.clickBlock ?? true)
    //        return;

    //    feildCardFrame.color = isMine ? new Color(0.5f, 0.5f, 1f) : Color.red;

    //    SingleEntityManager.instance.EntityMouseOver(this);

    //    if (Input.GetKeyDown(KeyCode.Mouse1))
    //        EnlargeCardManager.instance.Setup(card, true);
    //}
    //private void OnMouseExit()
    //{
    //    if (SingleManager.instance?.clickBlock ?? true)
    //        return;

    //    feildCardFrame.color = originColor;
    //}

    //private void OnMouseDown()
    //{
    //    if (clickBlock)
    //        return;
    //    if (SingleManager.instance?.clickBlock ?? true)
    //        return;

    //    if (isMine)
    //        SingleEntityManager.instance.EntityMouseDown(this);
    //}

    //private void OnMouseUp()
    //{
    //    if (clickBlock)
    //        return;
    //    if (SingleManager.instance?.clickBlock ?? true)
    //        return;
    //    SingleEntityManager.instance.EntityMouseUP(this);
    //}

    //private void OnMouseDrag()
    //{
    //    if (clickBlock)
    //        return;
    //    if (SingleManager.instance?.clickBlock ?? true)
    //        return;

    //    SingleEntityManager.instance.EntityMouseDrag();
    //}
    //#endregion


    //public void Damaged(int damage)
    //{
    //    Modifier modifier = new Modifier(-damage);
    //    card.Add_Modifier(modifier);
    //    UpdateStat();

    //}

    //public void MoveTransform(Vector3 pos, bool useDotween, float dotweenTime = 0)
    //{
    //    if (useDotween)
    //        transform.DOMove(pos, dotweenTime);
    //    else
    //        transform.position = pos;

    //    moveCount++;
    //}

    //private void Start()
    //{
    //    SingleTurnManager.OnTurnStarted += OnTurnStarted;
    //}

    //private void OnDestroy()
    //{
    //    SingleTurnManager.OnTurnStarted -= OnTurnStarted;
    //}

    //void OnTurnStarted(bool myTurn)
    //{
    //    if (isMine == myTurn)
    //        liveCount++;

    //    this.card.onTurnEnd();

    //    UpdateStat();
    //}

    //public void UpdateStat()
    //{
    //    if (card.name != null && card.cardType.card_category == CardCategory.Monster)
    //    {
    //        BattlePower_TMP.text = card.GetEffectiveValue("bp").ToString();
    //        if (card.GetEffectiveValue("bp") <= 0)
    //        {
    //            isDie = true;
    //            SingleEntityManager.instance.UpdateEntityState();
    //        }
    //    }
    //}

    //public int GetEffectiveValue(string stat)
    //{
    //    return card.GetEffectiveValue(stat);
    //}

    //public void SetGraveAfterFirst()
    //{
    //    this.transform.localScale = Vector3.zero;
    //}

    //public void Add_Apply_Effect(Ability ability)
    //{
    //    foreach (var effect in ability.effects)
    //    {
    //        effect.Resolve(this);
    //    }
    //}

    //public void ClickMark(bool isActive)
    //{
    //    arrow.gameObject.SetActive(isActive);
    //    arrow.transform.DOKill();
    //    arrow.DOFade(0, 1).SetEase(Ease.InSine).SetLoops(-1, LoopType.Restart);
    //}
}
