using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckSaveWindow : MonoBehaviour
{
    public DeckEditor deckEditor;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            deckEditor.Edit_Deck_Save();
        }
    }
}
