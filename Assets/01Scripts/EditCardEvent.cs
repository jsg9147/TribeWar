using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EditCardEvent : MonoBehaviour, IPointerClickHandler
{
    CardUI cardUI;
    CollectionManager collectionManager;

    public bool inDeck;

    private void Start()
    {
        cardUI = GetComponent<CardUI>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (cardUI.belong == Belong.Collection)
            {
                DeckEditor.instance.Card_Add_In_My_EditDeck(GetComponent<CollectionCard>());
            }
            else if (cardUI.belong == Belong.Deck)
            {
                DeckEditor.instance.Remove_Card_Of_Deck(cardUI);
            }
        }

        if(eventData.button == PointerEventData.InputButton.Left)
        {
            DeckEditor.instance.EnlargeCard_Setup(cardUI.card);
        }
    }
}
