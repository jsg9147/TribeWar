using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class ShopScreen : MonoBehaviour
{
    public Transform shopContent;
    public CardPackBuy cardPackPrefab;

    Dictionary<string, string> packInfo;

    [Header("»Ì±âÃ¢")]
    public GameObject drawCastScreen;
    public Transform card_Layout_group;

    private void Start()
    {

    }

    private void OnEnable()
    {
        Pack_Data_Setup();
    }

    void Pack_Data_Setup()
    {
        this.packInfo = WebMain.instance.web.packInfo;
        foreach(var pack in packInfo.Keys)
        {
            CardPackBuy cardPackBuy = Instantiate(cardPackPrefab, shopContent);
            cardPackBuy.transform.localScale = Vector3.one;
            cardPackBuy.transform.SetParent(shopContent);
            CardPack cardPack = WebMain.instance.web.cardPacks.Find(x => x.GetPackCode() == pack);
            cardPackBuy.Pack_Setup(cardPack, packInfo[pack], this);
        }
    }

    public void DrawScreen_On()
    {
        drawCastScreen.transform.DOScale(Vector3.one, 1);
    }

    public void DrawScreen_Off()
    {
        drawCastScreen.transform.DOKill();
        drawCastScreen.transform.localScale = Vector3.zero;
        DrawReset();
    }

    void DrawReset()
    {
        Transform[] childrenList = card_Layout_group.GetComponentsInChildren<Transform>();

        if (childrenList != null)
        {
            for (int i = 1; i < childrenList.Length; i++)
            {
                if (childrenList[i] != transform)
                    Destroy(childrenList[i].gameObject);
            }
        }
    }
}
