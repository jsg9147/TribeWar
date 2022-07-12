using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System;
using Newtonsoft.Json;
using Steamworks;
using System.Linq;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    JSONNode cardJson;
    public Dictionary<int, string> tutorial_cards;
    public Dictionary<int, string> ai_Deck_Dictionary;
    public int ai_Deck_Count;

    List<Card> cardDatas;
    public List<Card> userCollection;

    public List<Deck> playerDecks;

    public int lastUseDeck;

    public UserInfo userInfo;
    public string steamID;

    public Deck Select_Deck;

    public List<Card> selected_Deck_Cards;

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

        steamID = SteamUser.GetSteamID().ToString();
        cardDatas = new List<Card>();
        Select_Deck = new Deck();
        userInfo = new UserInfo();
        userCollection = new List<Card>();
        playerDecks = new List<Deck>();

        selected_Deck_Cards = new List<Card>();

        ai_Deck_Dictionary = new Dictionary<int, string>();

        TextAsset textData = Resources.Load("CardJson/CardData") as TextAsset;
        JSONNode nodeData = JSON.Parse(textData.text);

        SetCardDataJson(nodeData);

        Set_CardData();
        Load_Tutorial_Card();
        Load_User_Data();
        Temp_Set_Collection();
        Load_Deck();
    }
    void Set_CardData()
    {
        foreach (JSONNode card in cardJson)
        {
            Card cardData = new Card(card);
            cardDatas.Add(cardData);
        }
    }

    //임시 콜렉션 풀카드로 만들어주는 함수
    void Temp_Set_Collection()
    {
        foreach (var card in cardDatas)
        {
            for (int i = 0; i < 4; i++)
            {
                userCollection.Add(card);
            }
        }

        userCollection.OrderBy(x => x.name);
    }

    public void SetCardDataJson(JSONNode json) => cardJson = json;

    public Card Tutorial_Card(int index)
    {
        return CardData(tutorial_cards[index]);
    }

    public Card CardData(string card_id)
    {
        return cardDatas.Find(x => x.id == card_id);
    }

    public int CollectionCount(string card_id)
    {
        int count = 0;
        foreach (var card in userCollection)
        {
            if (card.id == card_id)
            {
                count++;
            }
        }
        return count;
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

    public List<Card> Load_AI_Deck()
    {
        int randomIndex = UnityEngine.Random.Range(0, ai_Deck_Count);

        string deckFile_name = "CardJson/AIDeck" + randomIndex.ToString();
        List<Card> ai_DeckCard = new List<Card>();

        TextAsset textAsset = Resources.Load(deckFile_name) as TextAsset;

        JSONNode root = JSON.Parse(textAsset.text);
        for (int i = 0; i < root.Count; i++)
        {
            JSONNode itemData = root[i];
            ai_Deck_Dictionary.Add(Int32.Parse(itemData["index"].Value), itemData["id"].Value);
        }

        foreach (string card_Id in ai_Deck_Dictionary.Values)
        {
            ai_DeckCard.Add(CardData(card_Id));
        }

        return ai_DeckCard;
    }

    public void Load_User_Data()
    {
        if (ES3.KeyExists(SteamUser.GetSteamID().ToString()))
        {
            string json = ES3.Load<string>(SteamUser.GetSteamID().ToString());
            userInfo = JsonUtility.FromJson<UserInfo>(json);
        }
        else
        {
            SaveUserInfo();
            string json = ES3.Load<string>(SteamUser.GetSteamID().ToString());
            userInfo = JsonUtility.FromJson<UserInfo>(json);
        }
    }
    public void SaveDeck(Deck deck)
    {
        if (playerDecks.Exists(x => x.index == deck.index))
        {
            int index = playerDecks.FindIndex(x => x.index == deck.index);
            playerDecks.RemoveAt(index);
        }
        playerDecks.Add(deck);

        string json = JsonConvert.SerializeObject(playerDecks.ToArray(), Formatting.None);

        ES3.Save(userInfo.UserID + "Deck", json);

        SaveUserInfo();
    }

    public void SaveDeckList()
    {
        string json = JsonConvert.SerializeObject(playerDecks.ToArray(), Formatting.None);

        ES3.Save(userInfo.UserID + "Deck", json);

        SaveUserInfo();
    }

    public void Load_Deck()
    {
        string deckJson;
        if (ES3.KeyExists(SteamUser.GetSteamID().ToString() + "Deck"))
        {
            if (ES3.Load<string>(SteamUser.GetSteamID().ToString() + "Deck") != null)
            {
                deckJson = ES3.Load<string>(SteamUser.GetSteamID().ToString() + "Deck");
                playerDecks = JsonConvert.DeserializeObject<List<Deck>>(deckJson);
            }
        }
    }

    public void SaveUserInfo()
    {
        userInfo.UserID = SteamUser.GetSteamID().ToString();
        string json = JsonUtility.ToJson(userInfo);
        
        ES3.Save(userInfo.UserID, json);
    }

    private void OnApplicationQuit()
    {
        SaveUserInfo();
    }

    public void SelectDeck(int index)
    {

    }
}
