using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Deck
{
    public int index;
    public string name;
    public List<string> cards;
    public string representCard;

    public Deck()
    {
        index = 0;
        name = "";
        cards = new List<string>();
        representCard = "";
    }

    public Deck(int index, string name, List<string> cards, string representCard)
    {
        this.index = index;
        this.name = name;
        this.cards = cards;
        this.representCard = representCard;
    }

    public Deck CloneDeck()
    {
        Deck clone = (Deck)this.MemberwiseClone();
        clone.index = new int();
        //clone.cardCount = new Dictionary<string, int>(this.cardCount);
        return clone;
    }

    public void SetCard(Card card, int count = 1)
    {
        //cardCount.Add(card.id, count);
        cards.Add(card.id);
    }

    public void DeckPaste(Deck deck)
    {
        this.name = deck.name;
        this.index = deck.index;
        this.representCard = deck.representCard;
    }

    public void Init()
    {
        //cardCount.Clear();
        cards = new List<string>();
        representCard = "";
    }

    public void Random_Represent_Card()
    {
        List<string> card_ids = new List<string>();

        foreach (var card_id in cards)
        {
            card_ids.Add(card_id);
        }
        card_ids = card_ids.Distinct().ToList();

        int randNum = Random.Range(0, card_ids.Count);

        representCard = card_ids[randNum];
    }

    public void RemoveCard(Card card)
    {
        int index = cards.FindIndex(x => x == card.id);
        cards.RemoveAt(index);
    }

    public int CardCount(string card_id)
    {
        int count = 0;
        foreach (var card in cards)
        {
            if (card == card_id)
            {
                count++;
            }
        }
        return count;
    }
}