using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManagerWindow : MonoBehaviour
{
    [SerializeField] DeckManager deckManager;

    private void OnEnable()
    {
        deckManager.DeckEditorListUpdate();
    }

    private void OnDisable()
    {
        deckManager.DeckListReset();
    }
}
