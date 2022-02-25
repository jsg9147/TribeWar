using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using TMPro;

public class MainScreen : MonoBehaviour
{
    public TMP_Text welcome_Text;

    private void Start()
    {
        if (!SteamManager.Initialized) { return; }
        WelcomeConnectText();
    }

    void WelcomeConnectText()
    {
        string user_name = SteamFriends.GetPersonaName();
        welcome_Text.text = user_name + "님 \n 접속을 환영합니다.";
    }
}
