using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class SingleHand : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public Sprite RookFrame;
    public Sprite BishopFrame;
    public Sprite QueenFrame;
    public Sprite MagicFrame;
    public Sprite BackFrame;

    public List<Sprite> Level_Sprite;

    [SerializeField] Image cardFrame;
    [SerializeField] Image classIcon;
    [SerializeField] TMP_Text nameTMP;
    [SerializeField] Image cardImage;
    [SerializeField] Image level_Icon;

    [SerializeField] TMP_Text cardTextTmp;
    [SerializeField] TMP_Text BattlePointTMP;

    bool isFront;
    public Card card;
    public PRS originPRS;

    public bool clickBlock;

    public void Setup(Card card, bool isFront)
    {
        this.isFront = isFront;
        if (card == null)
            this.isFront = false;
        if (this.isFront)
        {
            clickBlock = false;
            this.card = card;
            cardImage.sprite = card.sprite;
            nameTMP.text = card.name;
            if (card.cardType.card_category == CardCategory.Monster)
            {
                BattlePointTMP.text = "전투력 : " + card.GetBaseStat("bp").ToString();

                if (card.cardType.moveType == MoveType.Rook)
                    cardFrame.sprite = RookFrame;
                else if (card.cardType.moveType == MoveType.Bishop)
                    cardFrame.sprite = BishopFrame;
                else if (card.cardType.moveType == MoveType.Queen)
                    cardFrame.sprite = QueenFrame;
            }
            else
            {
                BattlePointTMP.text = "";

                cardFrame.sprite = MagicFrame;
            }
            if (cardTextTmp != null)
                cardTextTmp.text = card.card_text.ToString();
            if (classIcon != null)
                classIcon.sprite = card.cardType.typeIcon;

            level_Icon.sprite = Level_Sprite[card.cost];
        }
        else
        {
            cardFrame.sprite = BackFrame;
            classIcon.gameObject.SetActive(false);
            nameTMP.gameObject.SetActive(false);
            cardImage.gameObject.SetActive(false);
            cardTextTmp.gameObject.SetActive(false);
            BattlePointTMP.gameObject.SetActive(false);
            clickBlock = true;
        }

    }

    public void MoveTransform(PRS prs, bool useDotween, float dotweenTime = 0)
    {
        if (useDotween)
        {
            transform.DOMove(prs.pos, dotweenTime);
            transform.DORotateQuaternion(prs.rot, dotweenTime);
            transform.DOScale(prs.scale, dotweenTime);
        }
        else
        {
            //prs.pos = new Vector3(prs.pos.x, prs.pos.y + 15, prs.pos.z);
            transform.position = prs.pos;
            transform.rotation = prs.rot;
            transform.localScale = prs.scale;
        }
    }

    public void modulateAlpha(float value)
    {
        Color alpha = Color.white;

        alpha.a = value;

        cardFrame.GetComponent<Image>().color = alpha;
        cardImage.GetComponent<Image>().color = alpha;
    }

    public void changeColor(Color changeColor)
    {
        cardFrame.color = changeColor;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (SingleManager.instance?.clickBlock ?? true)
            return;
        if (clickBlock)
            return;

        if (isFront)
        {
            if (SingleCardManager.instance != null)
                SingleCardManager.instance.CardMouseOver(this);

            Color color = new Color32(146, 233, 255, 255);

            changeColor(color);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (SingleManager.instance?.clickBlock ?? true)
            return;
        if (clickBlock)
            return;

        if (isFront)
        {
            if (SingleCardManager.instance != null)
                SingleCardManager.instance.CardMouseExit(this);

            changeColor(Color.white);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (SingleManager.instance?.clickBlock ?? true)
            return;
        if (clickBlock)
            return;
        if (isFront)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                EnlargeCardManager.instance.Setup(this.card, isFront);
                return;
            }

            if (SingleCardManager.instance != null)
                SingleCardManager.instance.CardMouseDown(this);

            changeColor(Color.white);
            modulateAlpha(0.5f);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (SingleManager.instance?.clickBlock ?? true)
            return;
        if (clickBlock)
            return;

        if (isFront)
        {
            modulateAlpha(1f);
            if (SingleCardManager.instance != null)
                SingleCardManager.instance.CardMouseUp(this);
        }
    }
}
