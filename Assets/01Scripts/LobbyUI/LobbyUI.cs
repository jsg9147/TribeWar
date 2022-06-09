using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class LobbyUI : MonoBehaviour
{
    public static LobbyUI instance;

    public DeckEditor editor;

    public GameObject mainScreen;
    public GameObject playScreen;
    public GameObject myDeckScreen;
    public GameObject deckEditor;
    public GameObject shopScreen;
    public GameObject roomScreen;

    public GameObject matchLoadingWindow;
    public GameObject matchSuccessImage;

    [Header("도움말 UI")]
    public GameObject helpPanel;
    public GameObject help_Screen;
    public GameObject deckEditHelp_Screen;
    public GameObject dualScreenHelp_Screen;
    public GameObject cardInfoHelp_Screen;

    public Transform cardInfo_Main_Layout;

    private void Start()
    {
        MakeInstance();
        MainScreenOn();
    }

    #region gameUI

    void MakeInstance()
    {
        instance = this;
    }

    void ScreenClear()
    {
        mainScreen.SetActive(false);
        playScreen.SetActive(false);
        deckEditor.SetActive(false);
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

    public void QuickMatchButtonClick(bool isActive)
    {
        matchLoadingWindow.SetActive(isActive);
    }

    public void MatchSuccess()
    {
        matchSuccessImage.SetActive(true);
    }

    public void DeckEditorButtonClick()
    {
        ScreenClear();
        myDeckScreen.SetActive(true);
    }

    public void SelectEditMyDeck(Deck deck)
    {
        ScreenClear();
        deckEditor.SetActive(true);
        editor.Edit_Deck_Setup(deck);
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

    public void GetDrawCastScree()
    {

    }

    public void TutorialButton()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void ExitButton()
    {
        Application.Quit();
    }
    #endregion

    #region HelpUI
    void HelpScreen_Claer()
    {
        deckEditHelp_Screen.SetActive(false);
        dualScreenHelp_Screen.SetActive(false);
        cardInfoHelp_Screen.SetActive(false);
        help_Screen.SetActive(false);
        cardInfo_Main_Layout.DOLocalMoveX(0, 0f);
    }

    public void HelpScreen_SetActive(bool isActive)
    {
        ScreenClear();
        HelpScreen_Claer();
        helpPanel.SetActive(true);
        help_Screen.SetActive(isActive);

        if (isActive == false)
        {
            helpPanel.SetActive(false);
            MainScreenOn();
        }
    }

    public void Help_Deck_Edit_SetActive(bool isActive)
    {
        HelpScreen_Claer();
        deckEditHelp_Screen.SetActive(true);
    }

    public void Help_Dual_Screen_SetActive(bool isActive)
    {
        HelpScreen_Claer();
        dualScreenHelp_Screen.SetActive(true);
    }

    public void Help_Card_Info_SetActive(bool isActive)
    {
        HelpScreen_Claer();
        cardInfoHelp_Screen.SetActive(true);
    }

    public void CardInfo_Next_Page(bool isNext)
    {
        if (isNext)
            cardInfo_Main_Layout.DOLocalMoveX(-1920, 0.3f);
        else
            cardInfo_Main_Layout.DOLocalMoveX(0, 0.3f);
    }

    #endregion
}
