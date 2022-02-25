using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MyDeckRepresentCard : MonoBehaviour
{
    public Sprite RookFrame;
    public Sprite BishopFrame;
    public Sprite QueenFrame;
    public Sprite MagicFrame;

    [SerializeField] Image card_Frame;
    [SerializeField] Image card_Icon;
    [SerializeField] Image card_Image;
    [SerializeField] TMP_Text name_text;
    [SerializeField] TMP_Text card_text;
    [SerializeField] TMP_Text stat_text;

    [SerializeField] List<GameObject> costImageObject;
    Card card;

    public void Card_Setup(Card representCard)
    {
        this.card = representCard;
        
        name_text.text = card.name;
        card_Image.sprite = card.sprite;

        if (card.cardType.card_category == CardCategory.Monster)
        {
            stat_text.text = "ÀüÅõ·Â : " + card.GetBaseStat("BP").ToString();

            if (card.cardType.moveType == MoveType.Rook)
                card_Frame.sprite = RookFrame;
            else if (card.cardType.moveType == MoveType.Bishop)
                card_Frame.sprite = BishopFrame;
            else if (card.cardType.moveType == MoveType.Queen)
                card_Frame.sprite = QueenFrame;
            
        }
        if (card_Icon != null)
            card_Icon.sprite = card.cardType.typeIcon;
        if (card_text != null)
            card_text.text = card.card_text.ToString();

        for (int i = 0; i < card.cost; i++)
        {
            costImageObject[i].SetActive(true);
        }


    }
}
