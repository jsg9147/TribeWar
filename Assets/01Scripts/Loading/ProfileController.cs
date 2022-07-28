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

    float WinRate(int win, int lose)
    {
        float winRate;

        int total = win + lose;

        if (total == 0)
        {
            winRate = 0;
        }
        else
        {
            winRate = win / (lose + win) * 100f;
        }

        return winRate;
    }
    public void SetMyProfile(GamePlayer gamePlayer)
    {
        int win = gamePlayer.playerWin;
        int lose = gamePlayer.playerLose;

        float winRate = WinRate(win, lose);

        myAvatar.sprite = SetProfileIcon(SteamFriends.GetLargeFriendAvatar(gamePlayer.SteamID));
        //myAvatar.sprite = SetProfileIcon(SteamFriends.GetLargeFriendAvatar(new CSteamID(gamePlayer.SteamID)));

        string winRateStr = (lose == 0) ? "100.0" : winRate.ToString("F1");
        if (win == 0)
            winRateStr = "0.00";

        myName.text = gamePlayer.playerName;
        myRecord.text = win + "W " + lose + "L ( " + winRateStr + " % )";
    }

    public void SetOtherProfile(GamePlayer gamePlayer)
    {
        int win = gamePlayer.playerWin;
        int lose = gamePlayer.playerLose;

        float winRate = WinRate(win, lose);
        otherAvatar.sprite = SetProfileIcon(SteamFriends.GetLargeFriendAvatar(gamePlayer.SteamID));
        //otherAvatar.sprite = SetProfileIcon(SteamFriends.GetLargeFriendAvatar(new CSteamID(gamePlayer.SteamID)));

        string winRateStr = (lose == 0) ? "100.0" : winRate.ToString("F1");

        otherName.text = gamePlayer.playerName;
        otherRecord.text = win + "W " + lose + "L ( " + winRateStr + " % )";
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
