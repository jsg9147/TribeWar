using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class CardDatabase : MonoBehaviour
{
    public static CardDatabase instance;
    JSONNode cardJson;

    public void SetCardDataJson(JSONNode json) => cardJson = json;

    public Card CardData(string card_id)
    {
        List<Card> cardDatas = new List<Card>();
        foreach (JSONNode card in cardJson)
        {
            Card cardData = new Card(card);
            cardDatas.Add(cardData);
        }

        return cardDatas.Find(x => x.card_code == card_id);
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

}
