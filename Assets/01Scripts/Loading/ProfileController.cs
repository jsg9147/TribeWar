using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using TMPro;


public class ProfileController : MonoBehaviour
{
    public Image myAvatar;
    public TMP_Text myName;
    public TMP_Text myRecord;
    public TMP_Text myComment;

    public Image otherAvatar;
    public TMP_Text otherName;
    public TMP_Text otherRecord;
    public TMP_Text otherComment;

    public GameObject blackPanel;
    public void SetMyProfile(GamePlayer gamePlayer)
    {
        int win = gamePlayer.playerWin;
        int lose = gamePlayer.playerLose;

        float winRate = DataManager.instance.userInfo.WinRate();
        myAvatar.sprite = SetProfileIcon(gamePlayer.iImage);

        string winRateStr = (lose == 0) ? "100.0" : winRate.ToString("F1");
        if (win == 0)
            winRateStr = "0.00";

        myName.text = gamePlayer.playerName;
        myRecord.text = (int)win + "W " + (int)lose + "L ( " + winRateStr + " % )";
    }

    public void SetOtherProfile(GamePlayer gamePlayer)
    {
        int win = gamePlayer.playerWin;
        int lose = gamePlayer.playerLose;

        float winRate = gamePlayer.playerWinRate;
        otherAvatar.sprite = SetProfileIcon(gamePlayer.iImage);

        string winRateStr = (lose == 0) ? "100.0" : winRate.ToString("F1");
        if (win == 0)
            winRateStr = "0.00";

        otherName.text = gamePlayer.playerName;
        otherRecord.text = (int)win + "W " + (int)lose + "L ( " + winRateStr + " % )";
    }

    public void BlackPanelSetActive(bool isActive) => blackPanel.SetActive(isActive);

    Sprite SetProfileIcon(int iImage)
    {
        Texture2D steamAvatar = GetSteamImageAsTexture2D(iImage);

        if (steamAvatar == null)
            return null;

        Rect rect = new Rect(0, 0, steamAvatar.width, steamAvatar.height);
        return Sprite.Create(steamAvatar, rect, new Vector2(0.5f, 0.5f));
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
}
