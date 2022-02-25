using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyDeckScreen : MonoBehaviour
{
    [SerializeField] GameObject myDeckContent;
    public DeckManager deckManager;

    void MyDeckListUpdate()
    {
        deckManager.MyDeckListUpdate(myDeckContent, true);
    }

    private void OnEnable()
    {
        MyDeckListUpdate();
    }

    private void OnDisable()
    {
        foreach (Transform child in myDeckContent.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
