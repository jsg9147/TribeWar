using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeckEditor : MonoBehaviour
{
    public static DeckEditor instance;

    public int card_Limit_Count;

    [Header("UI")]
    public TMP_InputField deckTitleName_Input;
    public CardUI enlargeCardUI;
    public Button saveDeckButton;
    public GameObject edit_CollectCard_Content;
    public GameObject myDeckCardContent;
    public TMP_InputField searchField;

    [Header("Prefab")]
    public GameObject collectCardPrefab;
    public GameObject cardPrefab;

    [Header("그 밖")]
    public Button addDeckButton;
    public Deck myEditDeck;
    
    List<CollectionCard> collectionList;

    List<Card> myDeckCards;
    Dictionary<string, int> myEditCards;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //addDeckButton.onClick.AddListener(() =>
        //{
        //    ClearCardOfDeck();
        //    init();
        //    Give_Index_of_New_Deck();
        //    SetCardOfDeck(myEditDeck.index);
        //});

        searchField.onValueChanged.AddListener((string search) =>
        {
            CardSearch(search);
        });
    }

    public void init()
    {
        myEditDeck = new Deck();
        myDeckCards = new List<Card>();
        deckTitleName_Input.text = "";
    }

    // 새 덱 인덱스부여, 기존덱 번호 사라지면 당겨지는거 만들어야 하는지 확인 필요
    void Give_Index_of_New_Deck()
    {
        var myDeckList = WebMain.instance.web.myDeckList;
        int deckCount = myDeckList.Count;
        for (int i = 0; i <= deckCount; i++)
        {
            if (myDeckList.Exists(x => x.index == i) == false)
            {
                myEditDeck.index = i;
                break;
            }
        }
    }
    public void Input_Title(string title_Text)
    {
        if (title_Text == "")
            saveDeckButton.interactable = false;
        else
            saveDeckButton.interactable = true;
    }

    public void Edit_Deck_Setup(Deck _deck)
    {
        UIManager.instanse.DeckEditorWindowOn();
        myEditCards = new Dictionary<string, int>(WebMain.instance.web.userItems);
        collectionList = new List<CollectionCard>();
        Destroy_Card_Object_Of_Deck();

        myEditDeck = _deck.CloneDeck();
        deckTitleName_Input.text = myEditDeck.title;

        Create_EditWindow_CollectCard();

        if (myEditDeck != null)
        {
            foreach (var card_id in myEditDeck.cardCount.Keys)
            {
                for (int i = 0; i < myEditDeck.cardCount[card_id]; i++)
                {
                    Card card = CardDatabase.instance.CardData(card_id);
                    var editCard = Instantiate(cardPrefab, myDeckCardContent.transform);
                    editCard.GetComponent<CardUI>().Setup(card, true, Belong.Deck);
                    myDeckCards.Add(card);
                }
                myEditCards[card_id] = myEditCards[card_id] - myEditDeck.cardCount[card_id];
            }
            myDeckCardContent.GetComponent<FlexibleGrid>().SetFlexibleGrid();
        }
    }

    void Create_EditWindow_CollectCard()
    {   
        ClearCollectCard_Window(edit_CollectCard_Content);
        foreach (var card_id in WebMain.instance.web.userItems.Keys)
        {
            int collectionCount = WebMain.instance.web.userItems[card_id];

            foreach (var myCardID in myEditDeck.cardCount.Keys)
            {
                if (myCardID == card_id)
                {
                    collectionCount = collectionCount - myEditDeck.cardCount[card_id];
                    break;
                }
            }

            if (collectionCount != 0)
            {
                Card card = CardDatabase.instance.CardData(card_id);
                GameObject cardObj = Instantiate(collectCardPrefab, edit_CollectCard_Content.transform);
                cardObj.GetComponent<CollectionCard>().CardSetup(card, collectionCount);
                collectionList.Add(cardObj.GetComponent<CollectionCard>());
            }
        }
        edit_CollectCard_Content.GetComponent<FlexibleGrid>().SetFlexibleGrid();
    }

    void ClearCollectCard_Window(GameObject collectionContent)
    {
        Transform children = collectionContent.GetComponentInChildren<Transform>();

        foreach (Transform child in children)
        {
            if (child != collectionContent)
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void Remove_Card_Of_Deck(CardUI cardUI)
    {
        Card card = cardUI.card;

        myEditDeck.cardCount[card.card_code] -= 1;
        if (myEditDeck.cardCount[card.card_code] < 0)
        {
            myEditDeck.cardCount[card.card_code] = 0;
        }

        CollectionCard editCard = collectionList.Find(x => x.card.card_code == cardUI.card.card_code);
        if (editCard != null)
        {
            editCard.CardCountPlus();
        }
        else
        {
            GameObject cardObj = Instantiate(collectCardPrefab, edit_CollectCard_Content.transform);
            cardObj.GetComponent<CollectionCard>().CardSetup(card, 1);
            collectionList.Add(cardObj.GetComponent<CollectionCard>());
            edit_CollectCard_Content.GetComponent<FlexibleGrid>().SetFlexibleGrid();
        }
        myEditCards[card.card_code]++;
        myDeckCards.Remove(card);

        DestroyImmediate(cardUI.gameObject);
    }

    public void EnlargeCard_Setup(Card card)
    {
        enlargeCardUI.Setup(card);
    }

    // 덱 초기화
    public void Destroy_Card_Object_Of_Deck()
    {
        init();
        Give_Index_of_New_Deck();
        int i = 0;
        GameObject[] allChildren = new GameObject[myDeckCardContent.transform.childCount];

        foreach (Transform child in myDeckCardContent.transform)
        {
            allChildren[i] = child.gameObject;
            i++;
        }

        foreach (GameObject child in allChildren)
        {
            DestroyImmediate(child.gameObject);
        }
    }

    public void Deck_Clear()
    {
        Destroy_Card_Object_Of_Deck();
        Edit_Deck_Setup(new Deck());
    }

    public void Card_Add_In_My_EditDeck(CollectionCard collectionCard)
    {
        Card card = collectionCard.card;
        if(myEditDeck.cardIDs.Find(x => x == card.card_code) != null)
        {
            if (myEditDeck.cardCount[card.card_code] >= card_Limit_Count)
                return;

            myEditDeck.cardCount[card.card_code]++;

        }
        else
        {
            myEditDeck.SetCard(card);
        }

        myEditCards[card.card_code]--;
        collectionCard.CardCountMinus();

        if (myEditCards[card.card_code] <= 0)
        {
            collectionList.Remove(collectionCard);
            myEditCards[card.card_code] = 0;
            Destroy(collectionCard.gameObject);
        }

        var editCard = Instantiate(cardPrefab, myDeckCardContent.transform);
        editCard.GetComponent<CardUI>().Setup(card,true ,Belong.Deck);
        myDeckCards.Add(card);

        myDeckCardContent.GetComponent<FlexibleGrid>().SetFlexibleGrid();
    }

    public void Edit_Deck_Save()
    {
        if (saveDeckButton.interactable == false)
            return;

        foreach (var cardID in myEditDeck.cardCount.Keys)
        {
            StartCoroutine(WebMain.instance.web.UpdateDeckInfo(myEditDeck.index, myEditDeck.cardCount[cardID].ToString(), cardID.ToString()));
        }

        StartCoroutine(WebMain.instance.web.UpdateMyDeckTitleName(myEditDeck.index, deckTitleName_Input.text));

        if (WebMain.instance.web.myDeckList.Exists(x => x.index == myEditDeck.index))
        {
            Deck editDeck = WebMain.instance.web.myDeckList.Find(x => x.index == myEditDeck.index);
            editDeck.DeckPaste(myEditDeck);
            editDeck.title = deckTitleName_Input.text;
        }
        else
        {
            WebMain.instance.web.myDeckList.Add(myEditDeck);
        }
        deckTitleName_Input.text = "";

        UIManager.instanse.DeckManagerWindowOn();
    }

    #region 카드 서치 관련
    void CardSearch(string search)
    {
        foreach (var collectionCard  in collectionList)
        {
            if (collectionCard.card.name.Contains(search))
            {
                collectionCard.gameObject.SetActive(true);
            }
            else
            {
                collectionCard.gameObject.SetActive(false);
            }
        }
    }
    #endregion
}
