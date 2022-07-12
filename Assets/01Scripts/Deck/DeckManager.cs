using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckManager : MonoBehaviour
{
    [SerializeField] GameObject match_Deck_Select_Content;

    [SerializeField] Transform addDeckButton; // 덱 리스트 업데이트시 파괴 방지용
    [SerializeField] GameObject myDeckList_Content; // 초기화 시킬 방법이 안떠올라 사용

    public GameObject myDeckPrefab;
    public GameObject myDeckEditPrefab;

    List<MyDeck> myDeckList;
    public Button matchButton;

    public void MyDeckListUpdate(GameObject myDeckContent, bool _EditMyDeck)
    {
        if (DataManager.instance.playerDecks == null)
            return;

        foreach (Transform child in myDeckContent.transform)
        {
            if (child != addDeckButton)
                Destroy(child.gameObject);
        }
        myDeckList = new List<MyDeck>();
        List<Deck> deckList = DataManager.instance.playerDecks;
        
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
                matchButton.interactable = !(DataManager.instance.Select_Deck == null);
            }
        }

    }

    void MyDeckCreate(GameObject myDeckContent, Deck deck)
    {
        GameObject myDeckObj = Instantiate(myDeckPrefab, myDeckContent.transform);
        MyDeck myDeck = myDeckObj.GetComponent<MyDeck>();
        myDeck.Setup(deck);

        if (DataManager.instance.userInfo.Resently_Deck_index == deck.index)
        {
            myDeck.SelectDeck(true);
            DataManager.instance.Select_Deck = deck;
        }

        myDeckList.Add(myDeck);

        myDeckObj.GetComponent<Button>().onClick.AddListener(() =>
        {
            foreach (MyDeck allDeck in myDeckList)
            {
                allDeck.SelectDeck(false);
            }
            myDeck.SelectDeck(true);
            DataManager.instance.userInfo.Resently_Deck_index = deck.index;
            DataManager.instance.Select_Deck = deck;

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
            DarkTonic.MasterAudio.MasterAudio.PlaySound("Shuffle");
        });

        myDeck.deleteButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            DataManager.instance.playerDecks.Remove(deck);
            MyDeckListUpdate(myDeckList_Content, true);
            DataManager.instance.SaveDeckList();
        });
    }
}
