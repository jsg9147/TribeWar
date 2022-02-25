using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using Steamworks;

public class Web : MonoBehaviour
{
    public Items items;

    // 최근 사용한 덱 설정인데 이거 이용해서 게임 시작할때 어느덱으로 할지 정함
    public int recentlyDeckIndex;
    public List<Card> ItemDB = new List<Card>();
    public List<CardPack> cardPacks = new List<CardPack>();

    public List<Deck> myDeckList;

    public Dictionary<string, int> userItems = new Dictionary<string, int>(); // 아이템 아이디, 갯수
    public Dictionary<string, string> packInfo = new Dictionary<string, string>();

    Action<string> deckListCallback;
    Action<string> cardListOfDeckCallback;
    Action<string> myOwnCardListCallback;
    Action<string> packListCallback;
    Action<string> pack_CardList_Callback;

    string userID;
    public float win, lose = 0;
    public Deck selected_Deck;

    #region URL List
    
    public static string databaseServerURL = "http://58.127.49.117:8080/mydb/";
    
    string verCheckURL = "http://58.127.49.117:8080/";
    string get_CardDatabase_Json_URL = databaseServerURL + "CardDataExportJson.php";
    string loginURL = databaseServerURL + "Login.php";
    string get_Inven_CardIDs_URL = databaseServerURL + "GetInvenCardIDs.php";
    string sellItemURL = databaseServerURL + "SellItem.php";
    string get_Deck_Card_List_URL = databaseServerURL + "GetDeckCardList.php";
    string get_Deck_Title_URL = databaseServerURL + "GetDeckTitle.php";
    string updateUsersDeckURL = databaseServerURL + "UpdateUsersDeck.php";
    string updateUsersDeckInfoURL = databaseServerURL + "UpdateDeckInfo.php";
    string deleteDeckURL = databaseServerURL + "DeleteDeck.php";
    string getRecentlyUsedDeckURL = databaseServerURL + "GetRecentlyUsedDeck.php";
    string UpdateRecentlyUsedDeckURL = databaseServerURL + "UpdateRecentlyUsedDeck.php";
    string UsernameRegisterURL = databaseServerURL + "UsernameRegister.php";

    // 새로 추가
    string GetPackInfoURL = databaseServerURL + "GetPackInfo.php";
    string GetPackCardListURL = databaseServerURL + "GetPackCardList.php";
    string SetBuyCardURL = databaseServerURL + "SetBuyCard.php";

    #endregion
    void Start()
    {
        InitWeb();
    }

    void InitWeb()
    {
        myDeckList = new List<Deck>();
        StartCoroutine(GetVersion(verCheckURL));
        deckListCallback = (jsonArrayString) =>
        {
            StartCoroutine(GetUserDeckListRoutine(jsonArrayString));
        };

        cardListOfDeckCallback = (jsonArrayString) =>
        {
            StartCoroutine(CreateCardOfDeckRoutine(jsonArrayString));
        };

        myOwnCardListCallback = (jsonArrayString) =>
        {
            StartCoroutine(CreateUserItemsRoutine(jsonArrayString));
        };

        packListCallback = (jsonArrayString) =>
        {
            StartCoroutine(GetPackInfoRoutine(jsonArrayString));
        };


        pack_CardList_Callback = (jsonArrayString) =>
        {
            StartCoroutine(Get_Pack_CardList_Routine(jsonArrayString));
        };

        if (SteamManager.Initialized)
        {
            userID = SteamUser.GetSteamID().ToString();
        }
        StartCoroutine(GetRecentlyUsedDeck(SteamUser.GetSteamID().ToString()));
        StartCoroutine(GetCardDatabase());
    }

    IEnumerator GetVersion(string _uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(_uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = _uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(webRequest.downloadHandler.text); // webRequest.downloadHandler.text 이게 텍스트를 받아옴
                    break;
            }
        }
    }

    public IEnumerator Login(string _userID)
    {
        WWWForm form = new WWWForm();
        form.AddField("user_id", _userID);

        using (UnityWebRequest www = UnityWebRequest.Post(loginURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                WebMain.instance.userInfo.SetCredentials(_userID);

                if (www.downloadHandler.text.Contains("Wrong Credentials.") || www.downloadHandler.text.Contains("UserId does not exists."))
                {
                    Debug.Log("Try Again");
                }
                else if (www.downloadHandler.text.Contains("NewBeginer"))
                {
                    if (SteamManager.Initialized)
                    {
                        WebMain.instance.userInfo.SetID(SteamUser.GetSteamID().ToString());
                        StartCoroutine(WebMain.instance.web.Register(SteamFriends.GetPersonaName()));
                    }
                }
                else
                {
                    string jsonArrayString = www.downloadHandler.text;
                    JSONArray jsonArray = JSON.Parse(jsonArrayString) as JSONArray;

                    win = float.Parse(jsonArray[0].AsObject["win"]);
                    lose = float.Parse(jsonArray[0].AsObject["lose"]);

                    StartCoroutine(GetDeckInfo(_userID, deckListCallback));
                    StartCoroutine(GetDeckItemList(_userID, cardListOfDeckCallback));
                    StartCoroutine(GetUserOwnItems(_userID, myOwnCardListCallback));
                    StartCoroutine(GetPackList(packListCallback));
                    StartCoroutine(Get_Pack_CardList(pack_CardList_Callback));
                }
            }
        }
    }

    // 이름 변경 해줘야함 URL 도 jsg
    public IEnumerator Register(string username)
    {
        WWWForm form = new WWWForm();

        if (SteamManager.Initialized)
        {
            form.AddField("user_id", SteamUser.GetSteamID().ToString());
            form.AddField("name", username);
        }

        using (UnityWebRequest www = UnityWebRequest.Post(UsernameRegisterURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Debug.Log(www.downloadHandler.text);
                if (www.downloadHandler.text.Contains("Success"))
                {
                    WebMain.instance.userInfo.SetUsername(username);
                    //photonManager.Connect();
                }
                else
                {

                }
            }
        }
    }

    public IEnumerator GetCardDatabase()
    {
        WWWForm form = new WWWForm();

        List<Card> cardDatas = new List<Card>();

        using (UnityWebRequest www = UnityWebRequest.Post(get_CardDatabase_Json_URL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {

                JSONNode json = JSON.Parse(www.downloadHandler.text);

                foreach (JSONNode card in json)
                {
                    Card cardData = new Card(card);
                    cardDatas.Add(cardData);
                }
                CardDatabase.instance.SetCardDataJson(json);
            }
        }
    }

    IEnumerator CreateUserItemsRoutine(string jsonArrayString)
    {
        // Parsing json array string as an array
        JSONArray jsonArray = JSON.Parse(jsonArrayString) as JSONArray;
        if (jsonArray != null)
        {
            for (int i = 0; i < jsonArray.Count; i++)
            {
                string card_id = jsonArray[i].AsObject["card_id"];
                int card_count = jsonArray[i].AsObject["card_count"];

                userItems.Add(card_id, card_count);
            }
        }

        yield return null;
    }

    public IEnumerator GetUserOwnItems(string _userID, System.Action<string> callback)
    {
        WWWForm form = new WWWForm();

        form.AddField("user_id", _userID);

        using (UnityWebRequest www = UnityWebRequest.Post(get_Inven_CardIDs_URL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                string jsonArray = www.downloadHandler.text;

                callback(jsonArray);
            }
        }
    }


    #region DeckItem
    public IEnumerator GetDeckItemList(string _userID, System.Action<string> callback)
    {
        WWWForm form = new WWWForm();

        form.AddField("user_id", _userID);

        using (UnityWebRequest www = UnityWebRequest.Post(get_Deck_Card_List_URL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                string jsonArray = www.downloadHandler.text;

                callback(jsonArray);
            }
        }
    }

    IEnumerator GetUserDeckListRoutine(string jsonArrayString)
    {
        JSONArray jsonArray = JSON.Parse(jsonArrayString) as JSONArray;
        Deck deck;
        if (jsonArray != null)
        {
            for (int i = 0; i < jsonArray.Count; i++)
            {
                int deck_Index = jsonArray[i].AsObject["deck_index"];
                string deck_title = jsonArray[i].AsObject["deck_name"];
                string representCard = jsonArray[i].AsObject["represent_card"];

                if(myDeckList.Exists(x => x.index == deck_Index))
                {
                    deck = myDeckList.Find(x => x.index == deck_Index);
                }
                else
                {
                    deck = new Deck();
                    deck.index = deck_Index;
                    myDeckList.Add(deck);
                }

                deck.title = deck_title;
                deck.representCard = CardDatabase.instance.CardData(representCard);

            }
        }
        yield return null;
    }

    IEnumerator CreateCardOfDeckRoutine(string jsonArrayString)
    {
        JSONArray jsonArray = JSON.Parse(jsonArrayString) as JSONArray;

        if (jsonArray != null)
        {
            for (int i = 0; i < jsonArray.Count; i++)
            {
                string card_id = jsonArray[i].AsObject["card_id"];
                int card_count = jsonArray[i].AsObject["card_count"];
                int deck_index = jsonArray[i].AsObject["deck_index"];

                if(myDeckList.Exists(x => x.index == deck_index))
                {
                    var deck = myDeckList.Find(x => x.index == deck_index);
                    deck.SetCard(CardDatabase.instance.CardData(card_id), card_count);
                }
                else
                {
                    Deck deck = new Deck();
                    deck.index = deck_index;
                    deck.SetCard(CardDatabase.instance.CardData(card_id), card_count);
                    myDeckList.Add(deck);
                }
            }
        }
        yield return null;
    }

    #endregion

    #region 카드팩 데이터
    public IEnumerator Get_Pack_CardList(Action<string> callback)
    {
        WWWForm form = new WWWForm();

        using (UnityWebRequest www = UnityWebRequest.Post(GetPackCardListURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                string jsonArray = www.downloadHandler.text;
                callback(jsonArray);
                //Debug.Log(www.downloadHandler.text);
            }
        }
    }
    IEnumerator Get_Pack_CardList_Routine(string jsonArrayString)
    {
        JSONArray jsonArray = JSON.Parse(jsonArrayString) as JSONArray;
        CardPack cardPack;
        if (jsonArray != null)
        {
            for (int i = 0; i < jsonArray.Count; i++)
            {
                string pack_code = jsonArray[i].AsObject["pack_code"];
                int number = jsonArray[i].AsObject["card_number"];
                string rarity = jsonArray[i].AsObject["rarity"];

                cardPack = cardPacks.Find(x => x.GetPackCode() == pack_code);
                if (cardPack == null)
                {
                    cardPack = new CardPack();
                    cardPack.CardPack_Data_Setup(pack_code);
                    cardPacks.Add(cardPack);
                }

                cardPack.AddCard(number, rarity);
            }
        }

        yield return null;
    }

    public IEnumerator Set_Buy_Card(string card_code)
    {
        WWWForm form = new WWWForm();

        form.AddField("user_id", SteamUser.GetSteamID().ToString());
        form.AddField("card_code", card_code);

        using (UnityWebRequest www = UnityWebRequest.Post(SetBuyCardURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
            }
        }
    }

    #endregion


    public IEnumerator GetDeckInfo(string _userID, System.Action<string> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("user_id", _userID);
        using (UnityWebRequest www = UnityWebRequest.Post(get_Deck_Title_URL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                string jsonArray = www.downloadHandler.text;
                callback(jsonArray);
            }
        }
    }

    public IEnumerator UpdateDeckInfo(int _deckIndex, string _itemCount, string _itemID)
    {
        WWWForm form = new WWWForm();

        form.AddField("user_id", SteamUser.GetSteamID().ToString());
        form.AddField("deck_index", _deckIndex);
        form.AddField("card_count", _itemCount);
        form.AddField("card_id", _itemID);

        using (UnityWebRequest www = UnityWebRequest.Post(updateUsersDeckURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                //에러 메세지 뜨면 조치할수 있게 해야함
                Debug.Log(www.downloadHandler.text);
            }
        }
    }

    public IEnumerator GetPackList(Action<string> callback)
    {
        WWWForm form = new WWWForm();

        using (UnityWebRequest www = UnityWebRequest.Post(GetPackInfoURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                string jsonArray = www.downloadHandler.text;
                callback(jsonArray);
                //Debug.Log(www.downloadHandler.text);
            }
        }
    }
    IEnumerator GetPackInfoRoutine(string jsonArrayString)
    {
        JSONArray jsonArray = JSON.Parse(jsonArrayString) as JSONArray;
        if (jsonArray != null)
        {
            for (int i = 0; i < jsonArray.Count; i++)
            {
                string pack = jsonArray[i].AsObject["pack_code"];
                string name = jsonArray[i].AsObject["name"];

                packInfo.Add(pack, name);
            }
        }

        yield return null;
    }

    public IEnumerator DeleteDeck(string _deckIndex)
    {
        WWWForm form = new WWWForm();

        form.AddField("user_id", userID);
        form.AddField("deck_index", _deckIndex);

        using (UnityWebRequest www = UnityWebRequest.Post(deleteDeckURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
            }
        }
    }

    public IEnumerator GetRecentlyUsedDeck(string _userID)
    {
        WWWForm form = new WWWForm();

        form.AddField("user_id", _userID);

        using (UnityWebRequest www = UnityWebRequest.Post(getRecentlyUsedDeckURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                int index = 0;
                if(www.downloadHandler.text != null)
                {
                    index = Int32.Parse(www.downloadHandler.text);
                }
                recentlyDeckIndex = index;
            }
        }
    }

    public IEnumerator UpdateRecentlyUsedDeck(string user_id, int deck_index)
    {
        recentlyDeckIndex = deck_index;

        WWWForm form = new WWWForm();
        form.AddField("user_id", user_id);
        form.AddField("recently_use_deck", deck_index);

        using (UnityWebRequest www = UnityWebRequest.Post(UpdateRecentlyUsedDeckURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {

            }
        }
    }

    public IEnumerator UpdateMyDeckTitleName(int _deckIndex, string deckName)
    {
        WWWForm form = new WWWForm();

        form.AddField("user_id", SteamUser.GetSteamID().ToString());
        form.AddField("deck_index", _deckIndex);
        form.AddField("deck_name", deckName);

        using (UnityWebRequest www = UnityWebRequest.Post(updateUsersDeckInfoURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                // 마찬가지 에러 뜨면 조치할수 있게 해야함
                Debug.Log(www.downloadHandler.text);
            }
        }
    }


    // 실행 안되는 함수 
    // 현질 요소때문에 해놨는데 현질 요소를 구현 아예 안해놨음 php query문 부터 손봐야함
    // 참고영상
    // https://www.youtube.com/watch?v=3K9_S6RPHYA&list=PLTm4FjoXO7nfn0jB0Ig6UbZU1pUHSLhRU&index=14 
    public IEnumerator SellItem(string _itemID, string _userID)
    {
        WWWForm form = new WWWForm();
        form.AddField("user_id", _userID);
        form.AddField("card_id", _itemID);

        using (UnityWebRequest www = UnityWebRequest.Post(sellItemURL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
            }
        }
    }

    public void SetItems(Items items)
    {
        this.items = items;
    }
}
