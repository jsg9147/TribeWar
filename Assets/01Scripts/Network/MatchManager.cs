using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;

public class MatchManager : MonoBehaviour
{
    public static MatchManager instance;

    public string lobbyName;
    public GameObject localGamePlayerObject;
    public GamePlayer localGamePlayerScript;

    private void Awake()
    {
        MakeInstance();
    }

    void MakeInstance()
    {
        if (instance == null)
            instance = this;
    }
    public void FindLocalGamePlayer(GamePlayer localPlayer)
    {
        localGamePlayerObject = localPlayer.gameObject;
        localGamePlayerScript = localPlayer;
    }

    public void CreateInviteLobby()
    {
        Debug.Log("초대 게임 생성");
        ELobbyType newLobbyType = ELobbyType.k_ELobbyTypePublic;
        SteamLobby.instance.CreateNewLobby(newLobbyType, true); ;
    }

    public void CreateNewLobby()
    {
        Debug.Log("새로운 게임을 생성 하였습니다");
        ELobbyType newLobbyType = ELobbyType.k_ELobbyTypePublic;
        SteamLobby.instance.CreateNewLobby(newLobbyType, false);
    }

    public void MatchingStart()
    {
        Debug.Log("게임 리스트를 받아오는중 ...");
        SteamLobby.instance.GetListOfLobbies(false);
    }

    public void JoinCodeRoom()
    {
        SteamLobby.instance.GetListOfLobbies(true);
    }

    // 로비리스트 가져와서 목록 만드는 함수, 조금 수정해서 자동입장으로

    public void AutoJoinLobby(List<CSteamID> lobbyIDS, LobbyDataUpdate_t result, bool joinRoom)
    {
        for (int i = 0; i < lobbyIDS.Count; i++)
        {
            if (lobbyIDS[i].m_SteamID == result.m_ulSteamIDLobby)
            {
                Debug.Log("방 인원수: " + (SteamMatchmaking.GetNumLobbyMembers((CSteamID)lobbyIDS[i].m_SteamID)).ToString() + 
                    " max players: " + SteamMatchmaking.GetLobbyMemberLimit((CSteamID)lobbyIDS[i].m_SteamID).ToString());

                SteamLobby.instance.JoinLobby(lobbyIDS[i]);

                if(joinRoom)
                    LobbyUI.instance.CreateRoomButtonClick();

                break;
            }
        }
    }



    public void MatchingCancel()
    {
        SteamLobby.instance.StopMatching();
    }
}
