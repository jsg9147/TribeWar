using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScreen : MonoBehaviour
{
    [SerializeField] private GameObject deckListContent;
    [SerializeField] private GameObject ChatContent;
    public DeckManager deckManager;

    private void OnEnable()
    {
        MyDeckListUpdate();
        MessageReset();
    }

    void MessageReset()
    {
        Transform[] childrenList = ChatContent.GetComponentsInChildren<Transform>();

        if(childrenList != null)
        {
            for(int i = 1; i < childrenList.Length; i++)
            {
                if (childrenList[i] != transform)
                    Destroy(childrenList[i].gameObject);
            }
        }
    }

    void MyDeckListUpdate()
    {
        deckManager.MyDeckListUpdate(deckListContent, false);
    }
}
