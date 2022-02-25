using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexInfo
{
    public int IndexNumber;

    public IndexInfo(int IdNumber)
    {
        this.IndexNumber = IdNumber;
    }
}

public class Deck
{
    public IndexInfo IndexInfo;

    public int index;
    public string title;
    public Dictionary<string, int> cardCount = new Dictionary<string, int>();
    public Card representCard = null;
    public List<string> cardIDs = new List<string>();

    public Deck CloneDeck()
    {
        Deck clone = (Deck)this.MemberwiseClone();
        clone.index = new int();
        clone.cardCount = new Dictionary<string, int>(this.cardCount);
        clone.index = new IndexInfo(this.index).IndexNumber;
        clone.cardIDs = new List<string>(this.cardIDs);
        return clone;
    }

    public void SetCard(Card card, int count = 1)
    {
        cardCount.Add(card.card_code, count);
        cardIDs.Add(card.card_code);
    }

    public void DeckPaste(Deck deck)
    {
        this.title = deck.title;
        this.index = deck.index;
        this.cardCount = deck.cardCount;
        this.representCard = deck.representCard;

        foreach(string id in cardCount.Keys)
        {
            cardIDs.Add(id);
        }
    }

    public void DeckInit()
    {
        cardCount.Clear();
        cardIDs.Clear();
    }
}