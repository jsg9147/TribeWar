using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckManager : MonoBehaviour
{
    public Web web;
    [SerializeField] GameObject match_Deck_Select_Content;
    [SerializeField] GameObject deckSelectPrefab;

    [SerializeField] Transform addDeckButton; // 덱 리스트 업데이트시 파괴 방지용
    [SerializeField] GameObject myDeckList_Content; // 초기화 시킬 방법이 안떠올라 사용

    public GameObject myDeckPrefab;
    public GameObject myDeckEditPrefab;

    List<MyDeck> myDeckList;
    public Button matchButton;
    bool select;

    List<DeckSelection> deckList = new List<DeckSelection>();

    public void MyDeckListUpdate(GameObject myDeckContent, bool _EditMyDeck)
    {
        GameObject deckPrefab = _EditMyDeck ? myDeckEditPrefab : myDeckPrefab;

        if (web.myDeckList == null)
            return;

        foreach (Transform child in myDeckContent.transform)
        {
            if (child != addDeckButton)
                Destroy(child.gameObject);
        }

        myDeckList = new List<MyDeck>();
        List<Deck> deckList = web.myDeckList;

        foreach (Deck deck in deckList)
        {
            if (_EditMyDeck)
            {
                EditMyDeckCreate(myDeckContent, deck);
                myDeckContent.GetComponent<FlexibleGrid>().SetFlexibleGrid();
            }
            else
            {
                MyDeckCreate(myDeckContent, deck);
                myDeckContent.GetComponent<FlexibleGrid>().SetFlexibleGrid();
                matchButton.interactable = !(web.selected_Deck == null);
            }
        }

    }

    void MyDeckCreate(GameObject myDeckContent, Deck deck)
    {
        GameObject myDeckObj = Instantiate(myDeckPrefab, myDeckContent.transform);
        MyDeck myDeck = myDeckObj.GetComponent<MyDeck>();
        myDeck.Setup(deck);

        if (web.recentlyDeckIndex == deck.index)
        {
            myDeck.SelectDeck(true);
            web.selected_Deck = deck;
        }

        this.myDeckList.Add(myDeck);

        myDeckObj.GetComponent<Button>().onClick.AddListener(() =>
        {
            foreach (MyDeck allDeck in myDeckList)
            {
                allDeck.SelectDeck(false);
            }
            myDeck.SelectDeck(true);
            StartCoroutine(WebMain.instance.web.UpdateRecentlyUsedDeck(Steamworks.SteamUser.GetSteamID().ToString(), deck.index));
            web.selected_Deck = deck;

            matchButton.interactable = true;
        });
    }

    void EditMyDeckCreate(GameObject myDeckContent, Deck deck)
    {
        GameObject myDeckObj = Instantiate(myDeckEditPrefab, myDeckContent.transform);
        MyEditDeck myDeck = myDeckObj.GetComponent<MyEditDeck>();
        myDeck.MyDeckInfoSetup(deck);

        myDeckObj.GetComponent<Button>().onClick.AddListener(() =>
        {
            LobbyUI.instance.SelectEditMyDeck(deck);
        });

        myDeck.deleteButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            StartCoroutine(WebMain.instance.web.DeleteDeck(deck.index.ToString()));
            WebMain.instance.web.myDeckList.Remove(deck);
            MyDeckListUpdate(myDeckList_Content, true);
        });
    }

    public void Match_Deck_Select_List_Update()
    {
        select = false;
        List<Deck> deckList = WebMain.instance.web.myDeckList;
        foreach (Deck deck in deckList)
        {
            var deckSelectButton = Instantiate(deckSelectPrefab, match_Deck_Select_Content.transform);
            deckSelectButton.GetComponent<DeckSelection>().DeckInfoSet(deck.index, deck.name);
            deckSelectButton.GetComponent<DeckSelection>().IsMatchWindow(true);
            this.deckList.Add(deckSelectButton.GetComponent<DeckSelection>());

            deckSelectButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                Selected_Image_SetActive(deckSelectButton.GetComponent<DeckSelection>().deck);
                WebMain.instance.web.selected_Deck = deckSelectButton.GetComponent<DeckSelection>().deck;
                StartCoroutine(WebMain.instance.web.UpdateRecentlyUsedDeck(Steamworks.SteamUser.GetSteamID().ToString(), deck.index));
            });
        }

        if (this.deckList.Count > 0)
        {
            Deck recently_Deck = this.deckList.Find(x => x.deck.index == WebMain.instance.web.recentlyDeckIndex)?.deck;
            if (recently_Deck == null)
                return;
            Selected_Image_SetActive(recently_Deck);
            WebMain.instance.web.selected_Deck = recently_Deck;
        }

        matchButton.interactable = select;
    }

    void Selected_Image_SetActive(Deck selected_Deck)
    {
        foreach (var deck in deckList)
        {
            if (selected_Deck.index == deck.deck.index)
            {
                deck.Selected_This_Deck(true);
                select = true;
            }
            else
            {
                deck.Selected_This_Deck(false);
            }
        }

        matchButton.interactable = select;
    }

    public void DeckListReset()
    {
        foreach (var deckObjeck in deckList)
        {
            deckObjeck.DestroySelf();
        }
        deckList.Clear();
    }
}
