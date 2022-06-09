using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System;

public class CardDatabase : MonoBehaviour
{
    public static CardDatabase instance;

    JSONNode cardJson;

    public Dictionary<int, string> tutorial_cards;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Init();
    }

    void Init()
    {
        tutorial_cards = new Dictionary<int, string>();
        Load_Tutorial_Card();
    }

    public void SetCardDataJson(JSONNode json) => cardJson = json;

    public Card Tutorial_Card(int index) => CardData(tutorial_cards[index]);

    public Card CardData(string card_id)
    {
        List<Card> cardDatas = new List<Card>();
        foreach (JSONNode card in cardJson)
        {
            Card cardData = new Card(card);
            cardDatas.Add(cardData);
        }
        return cardDatas.Find(x => x.id == card_id);
    }

    public List<Card> GetCardDataList()
    {
        List<Card> cardDatas = new List<Card>();
        foreach (JSONNode card in cardJson)
        {
            Card cardData = new Card(card);
            cardDatas.Add(cardData);
        }
        return cardDatas;
    }

    void Load_Tutorial_Card()
    {
        TextAsset textAsset = Resources.Load("CardJson/TutorialCard") as TextAsset;

        JSONNode root = JSON.Parse(textAsset.text);
        for (int i = 0; i < root.Count; i++)
        {
            JSONNode itemData = root[i];
            tutorial_cards.Add(Int32.Parse(itemData["index"].Value), itemData["id"].Value);
        }
    }


}
