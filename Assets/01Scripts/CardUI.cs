using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CardUI : MonoBehaviour
{
    public Sprite RookFrame;
    public Sprite BishopFrame;
    public Sprite QueenFrame;
    public Sprite MagicFrame;
    public Sprite BackFrame;

    public List<Sprite> Level_Sprite_List;

    [SerializeField] Image cardFrame_Image;
    [SerializeField] Image classIcon_Image;
    [SerializeField] TMP_Text nameTMP;
    [SerializeField] Image cardImage;
    [SerializeField] TMP_Text cardTextTmp;
    [SerializeField] TMP_Text battlePower_TMP;
    [SerializeField] Image level_Icon;

    public Card card;
    bool isFront;

    public void Setup(Card card, bool isFront = true)
    {
        this.isFront = isFront;

        if (card == null)
            this.isFront = false;

        if (this.isFront)
        {
            this.card = card;
            cardImage.sprite = this.card.sprite;
            nameTMP.text = this.card.name;
            if (this.card.cardType.card_category == CardCategory.Monster)
            {
                battlePower_TMP.text = "전투력 : " + this.card.GetBaseStat("bp").ToString();

                if (this.card.cardType.moveType == MoveType.Rook)
                    cardFrame_Image.sprite = RookFrame;
                else if (this.card.cardType.moveType == MoveType.Bishop)
                    cardFrame_Image.sprite = BishopFrame;
                else if (this.card.cardType.moveType == MoveType.Queen)
                    cardFrame_Image.sprite = QueenFrame;
            }
            else
            {
                battlePower_TMP.text = "";

                cardFrame_Image.sprite = MagicFrame;
            }

            if (cardTextTmp != null)
            {
                cardTextTmp.text = this.card.card_text.ToString();
            }
            if (classIcon_Image != null)
            {
                classIcon_Image.sprite = this.card.cardType.typeIcon;
            }
            level_Icon.sprite = Level_Sprite_List[card.cost];

        }
        else
        {
            cardFrame_Image.sprite = BackFrame;
            classIcon_Image.gameObject.SetActive(false);
            nameTMP.gameObject.SetActive(false);
            cardImage.gameObject.SetActive(false);
            cardTextTmp.gameObject.SetActive(false);
            battlePower_TMP.gameObject.SetActive(false);
        }
    }
}
