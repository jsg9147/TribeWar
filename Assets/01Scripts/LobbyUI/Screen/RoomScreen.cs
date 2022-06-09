using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScreen : MonoBehaviour
{
    [SerializeField] private GameObject deckListContent;
    [SerializeField] private GameObject ChatContent;
    [SerializeField] private TMPro.TMP_Text readyButtonText;

    public DeckManager deckManager;

    private void OnEnable()
    {
        MyDeckListUpdate();
        MessageReset();
    }

    void MessageReset()
    {
        Transform[] childrenList = ChatContent.GetComponentsInChildren<Transform>();

        if (childrenList != null)
        {
            for (int i = 1; i < childrenList.Length; i++)
            {
                if (childrenList[i] != transform)
                    Destroy(childrenList[i].gameObject);
            }
        }
    }

    void MyDeckListUpdate()
    {
        foreach (Transform child in deckListContent.transform)
        {
            Destroy(child.gameObject);
        }
        deckManager.MyDeckListUpdate(deckListContent, false);
    }

    public void Ready_Button_Text_Change()
    {
        if (readyButtonText.text == "준 비")
            readyButtonText.text = "준비 해제";
        else
            readyButtonText.text = "준 비";
    }
}
