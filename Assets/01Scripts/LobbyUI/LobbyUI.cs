using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    public static LobbyUI instance;

    public GameObject mainScreen;
    public GameObject playScreen;
    public GameObject myDeckScreen;
    public GameObject deckEditorScreen;
    public GameObject shopScreen;
    public GameObject roomScreen;

    public GameObject matchLoadingWindow;
    private void Start()
    {
        MakeInstance();
        MainScreenOn();
    }

    void MakeInstance()
    {
        instance = this;
    }

    void ScreenClear()
    {
        mainScreen.SetActive(false);
        playScreen.SetActive(false);
        deckEditorScreen.SetActive(false);
        myDeckScreen.SetActive(false);
        shopScreen.SetActive(false);
        roomScreen.SetActive(false);
    }

    public void MainScreenOn()
    {
        ScreenClear();
        mainScreen.SetActive(true);
    }

    public void StartButtonClick()
    {
        ScreenClear();
        playScreen.SetActive(true);
    }

    public void QuickMatchButtonClick()
    {
        matchLoadingWindow.SetActive(true);
    }

    public void DeckEditorButtonClick()
    {
        ScreenClear();
        myDeckScreen.SetActive(true);
    }

    public void SelectEditMyDeck(Deck deck)
    {
        ScreenClear();
        deckEditorScreen.SetActive(true);
        deckEditorScreen.GetComponent<DeckEditor>().Edit_Deck_Setup(deck);
    }

    public void ShopButtonClick()
    {
        ScreenClear();
        shopScreen.SetActive(true);
    }

    public void CreateRoomButtonClick()
    {
        ScreenClear();
        roomScreen.SetActive(true);
    }

    public  void GetDrawCastScree()
    {

    }

    public void ExitButton()
    {
        Application.Quit();
    }

}
