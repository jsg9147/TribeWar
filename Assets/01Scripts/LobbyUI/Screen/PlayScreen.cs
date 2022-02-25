using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Steamworks;

public class PlayScreen : MonoBehaviour
{

    public Web web;

    public Image userIcon;
    public TMP_Text username_Text;
    public TMP_Text playerRecord_Text;
    public GameObject deckListContent;

    public DeckManager deckManager;

    private void OnEnable()
    {
        if (!SteamManager.Initialized)
            return;

        PlayerProfileUpdate();
        MyDeckListUpdate();
    }

    private void OnDisable()
    {
        
    }

    void PlayerProfileUpdate()
    {
        float winRate = web.win / (web.lose + web.win) * 100f;
        string winRateStr = (web.lose == 0) ? "100.0" : winRate.ToString("F1");
        if (web.win == 0)
            winRateStr = "0.00";

        username_Text.text = SteamFriends.GetPersonaName();
        playerRecord_Text.text = web.win + "½Â " + web.lose +  "ÆÐ\n( " + winRateStr + " % )"; // web¿¡¼­ Á¤º¸ °¡Á®¿À´Â Äõ¸® ÇØ³ö¾ßÇÔ
        Texture2D steamAvatar = GetSteamImageAsTexture2D(SteamFriends.GetMediumFriendAvatar(SteamUser.GetSteamID()));

        Rect rect = new Rect(0, 0, steamAvatar.width, steamAvatar.height);

        userIcon.sprite = Sprite.Create(steamAvatar, rect, new Vector2(0.5f, 0.5f));
    }

    public Texture2D GetSteamImageAsTexture2D(int iImage)
    {
        Texture2D ret = null;
        uint ImageWidth;
        uint ImageHeight;
        bool bIsValid = SteamUtils.GetImageSize(iImage, out ImageWidth, out ImageHeight);

        if (bIsValid)
        {
            byte[] Image = new byte[ImageWidth * ImageHeight * 4];

            bIsValid = SteamUtils.GetImageRGBA(iImage, Image, (int)(ImageWidth * ImageHeight * 4));
            if (bIsValid)
            {
                ret = new Texture2D((int)ImageWidth, (int)ImageHeight, TextureFormat.RGBA32, false, false);
                ret.LoadRawTextureData(Image);
                
                ret.Apply();
            }
        }
        return ret;
    }


    void MyDeckListUpdate()
    {
        deckManager.MyDeckListUpdate(deckListContent, false);
    }
}
