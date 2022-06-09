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
    public Button deleteButton;

    public void MyDeckInfoSetup(Deck _deck)
    {
        this.deck = _deck;
        Card represent = (deck.representCard == null) ? CardDatabase.instance.CardData("base-024") : deck.representCard;

        title_text.text = deck.name;

        representCard.Card_Setup(represent);
    }
}
