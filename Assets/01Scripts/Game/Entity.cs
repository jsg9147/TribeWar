using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class Entity : MonoBehaviour
{
    private const int PERMANENT = 0;

    public SpriteRenderer feildCardFrame;
    public SpriteRenderer cardSprite;
    public TMP_Text BattlePower_TMP;
    public SpriteRenderer arrow;
    public SpriteRenderer checkMark;

    public Sprite bishopFrame;
    public Sprite rookFrame;
    public Sprite queenFrame;

    public Card card;
    public Tile bottomTile
    {
        get
        {
            return MapManager.instance.mapData[coordinate.x, coordinate.y];
        }
    }

    public EntityBelong belong = EntityBelong.None;
    public bool isMine;
    public bool isDie;

    public bool attackable;

    public Vector3 transformPos
    {
        get
        {
            return transform.position;
        }
        set
        {
            MoveTransform(value, true, 0.5f);
        }
    }

    public int id;
    public Coordinate coordinate;

    public int liveCount;

    public int moveCount;
    public bool canTribute;

    List<Effect> timeEffects = new List<Effect>();

    int battlePower;

    Color originColor;
    Color textColor;

    public bool clickBlock;
    public void Setup(Card card, bool _isMine)
    {
        isMine = _isMine;
        coordinate = new Coordinate();


        if (card.cardType.card_category == CardCategory.Monster)
        {
            battlePower = card.GetBaseStat("bp");
        }

        if (isMine)
        {
            originColor = new Color(0, 146, 255);
            belong = EntityBelong.Player;
        }
        else
        {
            originColor = Color.red;//new Color(200, 40, 40);
            belong = EntityBelong.Enermy;
        }

        this.card = card.DeepCopy();
        cardSprite.sprite = this.card.sprite;
        moveCount = 0;
        liveCount = 0;
        canTribute = true;

        BattlePower_TMP.text = battlePower.ToString();
        ColorUtility.TryParseHtmlString("#00B8FF", out textColor);

        if (isMine == false)
        {
            cardSprite.transform.rotation = Quaternion.Euler(0, 0, 180);
            ColorUtility.TryParseHtmlString("#FF0007", out textColor);
        }
        BattlePower_TMP.color = textColor;
        feildCardFrame.color = originColor;

        clickBlock = false;
    }

    public void OriginColorChange() => feildCardFrame.color = originColor;

    #region Mouse activate

    private void OnMouseOver()
    {
        if (GameManager.instance.clickBlock)
            return;
        if (clickBlock)
            return;

        feildCardFrame.color = isMine ? new Color(0.5f, 0.5f, 1f) : Color.yellow;

        if (belong == EntityBelong.AI)
        {
            feildCardFrame.color = Color.black;
        }
        
        EntityManager.instance.EntityMouseOver(this);

        if (Input.GetKeyDown(KeyCode.Mouse1))
            EnlargeCardManager.instance.Setup(card, true);
    }
    private void OnMouseExit()
    {
        if (GameManager.instance.clickBlock)
            return;

        feildCardFrame.color = originColor;
        if (belong == EntityBelong.AI)
        {
            feildCardFrame.color = Color.yellow;
        }
        EntityManager.instance.EntityMouseExit(this);
    }

    private void OnMouseDown()
    {
        if (GameManager.instance.clickBlock)
            return;

        if (clickBlock)
            return;

        EntityManager.instance.EntityMouseDown(this);
    }

    private void OnMouseUp()
    {
        if (GameManager.instance.clickBlock)
            return;
        if (clickBlock)
            return;
        EntityManager.instance.EntityMouseUP(this);
    }

    private void OnMouseDrag()
    {
        if (GameManager.instance.clickBlock)
            return;
        if (clickBlock)
            return;
        EntityManager.instance.EntityMouseDrag();
    }
    #endregion


    public void Damaged(int damage)
    {
        Modifier modifier = new Modifier(-damage);
        card.Add_Modifier(modifier);
        UpdateStat();
    }

    public void MoveTransform(Vector3 pos, bool useDotween, float dotweenTime = 0)
    {
        if (useDotween)
            transform.DOMove(pos, dotweenTime);
        else
            transform.position = pos;

        moveCount++;
    }

    private void Start()
    {
        TurnManager.OnTurnStarted += OnTurnStarted;
    }

    private void OnDestroy()
    {
        TurnManager.OnTurnStarted -= OnTurnStarted;
    }

    void OnTurnStarted(bool myTurn)
    {
        if (isMine == myTurn)
            liveCount++;

        card.onTurnEnd();

        TurnEffect_Resolve();

        UpdateStat();
    }

    void TurnEffect_Resolve()
    {
        foreach (var effect in timeEffects)
        {
            if (effect.duration != PERMANENT)
            {
                effect.duration--;

                if (effect.duration <= 0)
                {
                    effect.Reverse(this);
                }
            }
        }
    }

    public void UpdateStat()
    {
        if (card.name != null && card.cardType.card_category == CardCategory.Monster)
        {
            BattlePower_TMP.text = card.GetEffectiveValue("bp").ToString();
            if (card.GetEffectiveValue("bp") <= 0)
            {
                isDie = true;
            }
        }
    }

    public int GetEffectiveValue(string stat)
    {
        return card.GetEffectiveValue(stat);
    }

    public void Add_Apply_Effect(Ability ability)
    {
        foreach (Effect effect in ability.effects)
        {
            effect.Resolve(this);
        }
    }

    public void ClickMark(bool isActive)
    {
        arrow.gameObject.SetActive(isActive);
        arrow.transform.DOKill();
        if (isActive)
        {
            arrow.DOFade(0, 1).SetEase(Ease.InSine).SetLoops(-1, LoopType.Restart);
        }
    }

    public void CheckMark(bool isActive)
    {
        if (checkMark == null)
            return;

        checkMark.gameObject.SetActive(isActive);
    }
}
