using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager instanse;

    [Header("��� ��ư")] 
    [SerializeField] Toggle playTabToggle; 
    [SerializeField] Toggle cardTabToggle; 
    [SerializeField] Button shopTabButton; 
    [SerializeField] Button optionButton; 

    [Header("ȭ���̵� ��ư")]
    [SerializeField] GameObject tabWindow;
    [SerializeField] GameObject playTab;
    [SerializeField] GameObject cardTab;

    [Header("�÷��� �� ��ư")]
    [SerializeField] Button playMatchButton;

    [Header("CardTab ��ư")]
    [SerializeField] Button collectionButton;
    [SerializeField] Button deckManagerButton;

    [Header("������")]
    [SerializeField] GameObject saveWindow;
    [SerializeField] Button saveButton;
    [SerializeField] Button saveCancelButton;

    [Header("��ġ UI")]
    [SerializeField] Button matchingButton;
    [SerializeField] Button matchCancelButton;
    [SerializeField] GameObject matchFindImage;
    [SerializeField] GameObject matchSuccessImage;

    [Header("â ����")]
    [SerializeField] GameObject collectionWindow;
    [SerializeField] GameObject enlargeCardWindow;
    [SerializeField] GameObject deckManagerWindow;
    [SerializeField] GameObject deckEditorWindow;
    [SerializeField] GameObject deckSelectWindow;
    [SerializeField] GameObject matchingWindow;
    [SerializeField] GameObject optionWindow;

    private void Awake()
    {
        instanse = this;
    }

    private void Start()
    {
        ButtonAddListener();
    }

    private void OnApplicationQuit()
    {
        // ���� ����� ��������
    }

    void ButtonAddListener()
    {
        playTabToggle.onValueChanged.AddListener((bool isOn) =>
        {
            playTab.SetActive(isOn);
        });

        cardTabToggle.onValueChanged.AddListener((bool isOn) =>
        {
            cardTab.SetActive(isOn);
        });

        collectionButton.onClick.AddListener(() =>
        {
            WindowsClear();
            collectionWindow.SetActive(true);
            cardTabToggle.isOn = false;
        });

        deckManagerButton.onClick.AddListener(() =>
        {
            DeckManagerWindowOn();
            cardTabToggle.isOn = false;
        });

        saveButton.onClick.AddListener(() =>
        {
            saveWindow.SetActive(true);
        });

        saveCancelButton.onClick.AddListener(() =>
        {
            saveWindow.SetActive(false);
        });

        playMatchButton.onClick.AddListener(() =>
        {
            WindowsClear();
            deckSelectWindow.SetActive(true);
        });

        matchingButton.onClick.AddListener(() =>
        {
            matchingWindow.SetActive(true);
        });

        matchCancelButton.onClick.AddListener(() =>
        {
            matchingWindow.SetActive(false);
        });

        optionButton.onClick.AddListener(() =>
        {
            WindowsClear();
            optionWindow.SetActive(true);
        });
    }

    public void WindowsClear()
    {
        cardTab.SetActive(false);
        playTab.SetActive(false);
        collectionWindow.SetActive(false);
        enlargeCardWindow.SetActive(false);
        deckManagerWindow.SetActive(false);
        deckEditorWindow.SetActive(false);
        saveWindow.SetActive(false);
        deckSelectWindow.SetActive(false);
        optionWindow.SetActive(false);
    }

    public void EnlargeCardOn(Card card)
    {
        enlargeCardWindow.SetActive(true);
        enlargeCardWindow.GetComponent<EnlargeCardUI>().SetCardData(card);
    }

    public void DeckEditorWindowOn()
    {
        WindowsClear();
        deckEditorWindow.SetActive(true);
    }

    public void DeckManagerWindowOn()
    {
        WindowsClear();
        deckManagerWindow.SetActive(true);
    }

    public void DeckSelectWindowActive()
    {
        WindowsClear();
        deckSelectWindow.SetActive(true);
    }

    public void MatchingSuccess()
    {
        matchFindImage.SetActive(false);
        matchSuccessImage.SetActive(true);

        matchCancelButton.interactable = false;
    }
}
