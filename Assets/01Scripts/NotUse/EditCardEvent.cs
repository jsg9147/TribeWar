using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EditCardEvent : MonoBehaviour, IPointerClickHandler
{
    public 
    CardUI cardUI;
    CollectionManager collectionManager;
    CollectionCard collectionCard;

    public bool inCollector;

    private void Start()
    {
        Init();

        cardUI = GetComponent<CardUI>();
    }

    void Init()
    {
        if (GetComponent<CollectionCard>())
        {
            inCollector = true;
            collectionCard = GetComponent<CollectionCard>();
        }
        else
        {
            inCollector = false;
        }
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (inCollector)
            {
                DeckEditor.instance.Card_Add_In_My_EditDeck(collectionCard);
            }
            else
            {
                DeckEditor.instance.Remove_Card_Of_Deck(cardUI);
            }
        }

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            DeckEditor.instance.EnlargeCard_Setup(cardUI.card);
        }
    }
}
