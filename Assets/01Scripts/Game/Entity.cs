using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class Entity : MonoBehaviour
{
    [SerializeField] SpriteRenderer feildCardFrame;
    [SerializeField] SpriteRenderer cardSprite;
    [SerializeField] TMP_Text BattlePower_TMP;

    public Sprite bishopFrame;
    public Sprite rookFrame;
    public Sprite queenFrame;

    public Card card;
    public Tile bottomTile
    {
        get { return MapManager.instance.mapData[coordinate.x, coordinate.y]; }
        set {; }
    }

    public bool isMine;
    public bool isDie;

    public bool attackable;

    public Vector3 transformPos
    {
        get { return transform.position; }
        set { MoveTransform(value, true, 0.5f); }
    }

    public int id;
    public Coordinate coordinate;

    public int liveCount;

    public int moveCount;
    public bool canTribute;

    int battlePower;

    public void Setup(Card card)
    {
        coordinate = new Coordinate();

        if(card.cardType.card_category == CardCategory.Monster)
        {
            battlePower = card.GetBaseStat("BP");
        }

        switch (card.cardType.moveType)
        {
            case MoveType.Rook:
                feildCardFrame.sprite = rookFrame;
                break;
            case MoveType.Bishop:
                feildCardFrame.sprite = bishopFrame;
                break;
            case MoveType.Queen:
                feildCardFrame.sprite = queenFrame;
                break;

            default:
                feildCardFrame.sprite = queenFrame;
                break;
        }

        this.card = card;
        cardSprite.sprite = this.card.sprite;
        BattlePower_TMP.text = battlePower.ToString();
        moveCount = 0;
        liveCount = 0;
        canTribute = true;

        if (isMine == false)
        {
            cardSprite.transform.rotation = Quaternion.Euler(0, 0, 180);
            BattlePower_TMP.alignment = TextAlignmentOptions.MidlineRight;
        }
    }

    #region Mouse activate

    private void OnMouseOver()
    {
        if (GameManager.instance?.clickBlock ?? true)
            return;

        feildCardFrame.color = isMine ? new Color(0.5f, 0.5f, 1f) : Color.red;

        EntityManager.instance.EntityMouseOver(this);

        if(Input.GetKeyDown(KeyCode.Mouse1))
            EnlargeCardManager.instance.Setup(card, true);
    }
    private void OnMouseExit()
    {
        if (GameManager.instance?.clickBlock ?? true)
            return;

        feildCardFrame.color = new Color(1f, 1f, 1f);
    }

    private void OnMouseDown()
    {
        if (GameManager.instance?.clickBlock ?? true)
            return;

        if (isMine)
            EntityManager.instance.EntityMouseDown(this);
    }

    private void OnMouseUp()
    {
        if (GameManager.instance?.clickBlock ?? true)
            return;
        EntityManager.instance.EntityMouseUP(this);
    }

    private void OnMouseDrag()
    {
        if (GameManager.instance?.clickBlock ?? true)
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

        UpdateStat();
    }

    public void UpdateStat()
    {
        if (card.name != null && card.cardType.card_category == CardCategory.Monster)
        {
            BattlePower_TMP.text = card.GetEffectiveValue("BP").ToString();
            if (card.GetEffectiveValue("BP") <= 0)
            {
                isDie = true;
            }
        }
    }

    public int GetEffectiveValue(string stat)
    {
        return card.GetEffectiveValue(stat);
    }

    public void OppenentFeildCardColor(Color color)
    {
        feildCardFrame.color = color;
    }

    public void SetGraveAfterFirst()
    {
        this.transform.localScale = Vector3.zero;
    }

    public void Add_Apply_Effect(Ability ability)
    {
        ability.effect.Resolve(this);
    }
}
