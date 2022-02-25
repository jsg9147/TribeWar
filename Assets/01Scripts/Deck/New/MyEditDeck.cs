using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MyEditDeck : MonoBehaviour
{
    public Deck deck;
    public MyDeckRepresentCard representCard;
    public TMP_Text title_text;

    public void MyDeckInfoSetup(Deck _deck)
    {
        this.deck = _deck;
        Card represent = (deck.representCard == null) ? CardDatabase.instance.CardData("BASE-001") : deck.representCard;

        title_text.text = deck.title;

        representCard.Card_Setup(represent);
    }
}
