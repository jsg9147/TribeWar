using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;

public class HandCard : MonoBehaviour
{
    [SerializeField] SpriteRenderer cardFront;
    [SerializeField] SpriteRenderer character;
    [SerializeField] TMP_Text nameTMP;
    [SerializeField] TMP_Text attackTMP;
    [SerializeField] TMP_Text defenseTMP;
    [SerializeField] TMP_Text monsterTypeTMP;
    [SerializeField] TMP_Text explanationTMP;
    [SerializeField] TMP_Text costTMP;
    [SerializeField] Sprite cardBack;
    [SerializeField] SpriteRenderer attackIcon;
    [SerializeField] SpriteRenderer defenseIcon;
    [SerializeField] SpriteRenderer typeIcon;

    public Card card;
    bool isFront;
    public PRS originPRS;

    //public void Setup(Card item, bool isFront)
    //{
    //    this.isFront = isFront;

    //    if (item == null)
    //        this.isFront = false;

    //    if(this.isFront)
    //    {
    //        this.card = item;
    //        character.sprite = card.sprite;
    //        nameTMP.text = card.name;
    //        if(card.cardType.card_Class == CardClass.Monster)
    //        {
    //            attackTMP.text = card.GetBaseStat("attack").ToString();
    //            defenseTMP.text = card.GetBaseStat("defense").ToString();
    //        }
    //        else
    //        {
    //            attackTMP.text = "";
    //            defenseTMP.text = "";
    //            attackIcon.sprite = null;
    //            defenseIcon.sprite = null;
    //        }
            
    //        costTMP.text = card.cost.ToString();

    //        if (explanationTMP != null)
    //            explanationTMP.text = card.card_text.ToString();
    //        if(typeIcon != null)
    //            typeIcon.sprite = card.cardType.typeIcon;
    //        if(monsterTypeTMP != null)
    //            monsterTypeTMP.text = card.TribeStr();

    //        cardFront = GetComponent<SpriteRenderer>();
    //    }
    //    else
    //    {
    //        cardFront.sprite = cardBack;
    //        nameTMP.text = "";
    //        attackTMP.text = "";
    //        defenseTMP.text = "";
    //        explanationTMP.text = "";
    //        attackIcon.sprite = null;
    //        defenseIcon.sprite = null;
    //    }
    //}

    //#region Mouse Interact
    //private void OnMouseOver()
    //{
    //    if (GameManager.instance?.clickBlock ?? true)
    //        return;

    //    if (isFront)
    //    {
    //        if (CardManager.instance != null)
    //            CardManager.instance.CardMouseOver(this);

    //        changeColor( Color.blue);


    //        if (Input.GetKeyDown(KeyCode.Mouse1))
    //        {
    //            EnlargeCardManager.instance.Setup(this.card, isFront);
    //        }
    //    }
    //}

    //private void OnMouseExit()
    //{
    //    if (GameManager.instance?.clickBlock ?? true)
    //        return;

    //    if (isFront)
    //    {
    //        if (CardManager.instance != null)
    //            CardManager.instance.CardMouseExit(this);

    //        changeColor(Color.white);
    //    }
    //}

    //private void OnMouseDown()
    //{
    //    if (GameManager.instance?.clickBlock ?? true)
    //        return;

    //    if (isFront)
    //    {
    //        if (CardManager.instance != null)
    //            CardManager.instance.CardMouseDown(this);

    //        changeColor(Color.white);
    //    }   
    //}
    //private void OnMouseUp()
    //{
    //    if (GameManager.instance?.clickBlock ?? true)
    //        return;

    //    if (isFront)
    //    {
    //        modulateAlpha(1f);
    //        if (CardManager.instance != null)
    //            CardManager.instance.CardMouseUp(this);
    //    }
    //}

    //#endregion 

    //public void MoveTransform(PRS prs, bool useDotween, float dotweenTime = 0)
    //{
    //    if (useDotween)
    //    {
    //        transform.DOMove(prs.pos, dotweenTime);
    //        transform.DORotateQuaternion(prs.rot, dotweenTime);
    //        transform.DOScale(prs.scale, dotweenTime);
    //    }
    //    else
    //    {
    //        transform.position = prs.pos;
    //        transform.rotation = prs.rot;
    //        transform.localScale = prs.scale;
    //    }
    //}

    //public void modulateAlpha(float value)
    //{
    //    Color alpha = Color.white;

    //    alpha.a = value;

    //    character.GetComponent<SpriteRenderer>().color = alpha;
    //    nameTMP.GetComponent<TextMeshPro>().color = alpha;
    //    defenseTMP.GetComponent<TextMeshPro>().color = alpha;
    //    attackTMP.GetComponent<TextMeshPro>().color = alpha;
    //}

    //public void changeColor(Color changeColor)
    //{
    //    cardFront.color = changeColor;
    //}
}
