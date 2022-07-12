using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectionCard : MonoBehaviour
{
    public TMP_Text card_Count_text;
    public CardUI cardUI;
    public int cardCount;

    public Card card;



    public void CardSetup(Card _card, int count)
    {
        this.card = _card;
        cardCount = count;
        card_Count_text.text = "×" + count.ToString();
        cardUI.Setup(card);
    }


    public void CardCountPlus()
    {
        cardCount++;
        card_Count_text.text = "×" + cardCount.ToString();
    }

    public void CardCountMinus()
    {
        cardCount--;
        card_Count_text.text = "×" + cardCount.ToString();
    }

    private void OnDestroy()
    {
        GetComponentInParent<FlexibleGrid>()?.SetFlexibleGrid();
    }
}
