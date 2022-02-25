using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckSelectWindow : MonoBehaviour
{
    [SerializeField] DeckManager deckManager;
    private void OnEnable()
    {
        deckManager.Match_Deck_Select_List_Update();
    }

    private void OnDisable()
    {
        deckManager.DeckListReset();
    }
}
