using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class Hand : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public Sprite RookFrame;
    public Sprite BishopFrame;
    public Sprite QueenFrame;
    public Sprite MagicFrame;
    public Sprite BackFrame;

    [SerializeField] Image cardFrame;
    [SerializeField] Image classIcon;
    [SerializeField] TMP_Text nameTMP;
    [SerializeField] Image cardImage;
    [SerializeField] List<GameObject> costImageObject;
    [SerializeField] TMP_Text tribeTMP;
    [SerializeField] TMP_Text cardTextTmp;
    [SerializeField] TMP_Text attackTMP;
    [SerializeField] TMP_Text defenseTMP;

    public Card card;
    bool isFront;
    public PRS originPRS;

    public void Setup(Card item, bool isFront)
    {
        this.isFront = isFront;

        if (item == null)
            this.isFront = false;

        if (this.isFront)
        {
            this.card = item;
            cardImage.sprite = card.sprite;
            nameTMP.text = card.name;
            if (card.cardType.card_category == CardCategory.Monster)
            {
                attackTMP.text = "ÀüÅõ·Â : " + card.GetBaseStat("BP").ToString();

                if (card.cardType.moveType == MoveType.Rook)
                    cardFrame.sprite = RookFrame;
                else if (card.cardType.moveType == MoveType.Bishop)
                    cardFrame.sprite = BishopFrame;
                else if (card.cardType.moveType == MoveType.Queen)
                    cardFrame.sprite = QueenFrame;
            }
            else
            {
                attackTMP.text = "";
                defenseTMP.text = "";

                cardFrame.sprite = MagicFrame;
            }

            for(int i = 0; i < card.cost; i++)
            {
                costImageObject[i].SetActive(true);
            }

            if (cardTextTmp != null)
                cardTextTmp.text = card.card_text.ToString();
            if (classIcon != null)
                classIcon.sprite = card.cardType.typeIcon;
            if (tribeTMP != null)
                tribeTMP.text = card.TribeStr();

        }
        else
        {
            cardFrame.sprite = BackFrame;
            classIcon.gameObject.SetActive(false);
            nameTMP.gameObject.SetActive(false);
            cardImage.gameObject.SetActive(false);
            tribeTMP.gameObject.SetActive(false);
            cardTextTmp.gameObject.SetActive(false);
            attackTMP.gameObject.SetActive(false);
            defenseTMP.gameObject.SetActive(false);
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
        if (GameManager.instance?.clickBlock ?? true)
            return;

        if (isFront)
        {
            if (CardManager.instance != null)
                CardManager.instance.CardMouseOver(this);

            Color color = new Color32(146,233,255,255);

            changeColor(color);

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                EnlargeCardManager.instance.Setup(this.card, isFront);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (GameManager.instance?.clickBlock ?? true)
            return;

        if (isFront)
        {
            if (CardManager.instance != null)
                CardManager.instance.CardMouseExit(this);

            changeColor(Color.white);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.instance?.clickBlock ?? true)
            return;

        if (isFront)
        {
            if (CardManager.instance != null)
                CardManager.instance.CardMouseDown(this);

            changeColor(Color.white);
            modulateAlpha(0.5f);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (GameManager.instance?.clickBlock ?? true)
            return;

        if (isFront)
        {
            modulateAlpha(1f);
            if (CardManager.instance != null)
                CardManager.instance.CardMouseUp(this);
        }
    }
}
