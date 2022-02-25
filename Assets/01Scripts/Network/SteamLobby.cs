using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Steamworks;
using Mirror;

public class SteamLobby : MonoBehaviour
{
    public static SteamLobby instance;

    NetworkManager networkManager;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;
    protected Callback<LobbyMatchList_t> Callback_lobbyList;
    protected Callback<LobbyDataUpdate_t> Callback_lobbyInfo;

    public List<CSteamID> lobbyIDS = new List<CSteamID>();
    public CSteamID currentLobby;

    private const string HostAddressKey = "HostAdress";
    private const string GameKey = "MSGgames";
    private const string GameValue = "Tribu_War_In_MSGgames";

    public TMP_InputField joinCode_InputText;
    public string inviteCode;
    public TMP_Text inviteCode_Text;

    bool inviteRoom;
    bool _joinCodeRoom;
    bool _createLobby;

    struct LobbyMetaData
    {
        public string m_Key;
        public string m_Value;
    }

    struct LobbyMembers
    {
        public CSteamID m_SteamID;
        public LobbyMetaData[] m_Data;
    }
    struct Lobby
    {
        public CSteamID m_SteamID;
        public CSteamID m_Owner;
        public LobbyMembers[] m_Members;
        public int m_MemberLimit;
        public LobbyMetaData[] m_Data;
    }

    private void Start()
    {
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();

        if (!SteamManager.Initialized) { return; }

        MakeInstance();
        CSteamID steamID = SteamUser.GetSteamID();
        StartCoroutine(WebMain.instance.web.Login(steamID.m_SteamID.ToString()));

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        Callback_lobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbiesList);
        Callback_lobbyInfo = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyInfo);
    }

    void CreateInviteCode()
    {
        inviteCode = "";
        for (int i = 0; i < 8; i++)
        {
            bool randomInt = (Random.value > 0.5f);
            if (randomInt)
                inviteCode = inviteCode + (char)Random.Range(65, 90);
            else
                inviteCode = inviteCode + (char)Random.Range(48, 57);
        }

        inviteCode_Text.text = inviteCode;
    }

    void MakeInstance()
    {
        if (instance == null)
            instance = this;
    }


    public void HostLobby()
    {
        inviteRoom = false;
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 2);
    }

    public void JoinLobby(CSteamID lobbyId)
    {
        Debug.Log("로비에 참가 신청 : " + lobbyId.ToString());
        SteamMatchmaking.JoinLobby(lobbyId);
    }

    public void LeaveLobby()
    {
        Debug.Log("로비를 나갔습니다");
        if(currentLobby != null)
        {
            SteamMatchmaking.LeaveLobby(currentLobby);
        }

        currentLobby = new CSteamID();
    }

    public void GetListOfLobbies(bool joinCodeRoom)
    {
        if (lobbyIDS.Count > 0)
            lobbyIDS.Clear();

        _joinCodeRoom = joinCodeRoom;

        SteamMatchmaking.AddRequestLobbyListFilterSlotsAvailable(1);
        SteamMatchmaking.RequestLobbyList();
    }


    public void OnLobbyCreated(LobbyCreated_t callback)
    {
        string msg = "로비 생성 : ";

        if(callback.m_eResult != EResult.k_EResultOK)
        {
            return;
        }
        if(networkManager.isNetworkActive == false)
        {
            networkManager.StartHost();
        }

        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby), 
            HostAddressKey, 
            SteamUser.GetSteamID().ToString());

        //주소, 키값, 키의 벨류 순으로 추정

        if(inviteRoom)
        {
            SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            inviteCode,
            GameValue);

            SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            GameKey,
            inviteCode);
        }
        else
        {
            SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            GameKey,
            GameValue);
        }

        msg = inviteRoom ? msg + inviteCode : msg + GameKey;

        Debug.Log(msg);
        
        _createLobby = true;
    }


    //초대 받았을때 동작.
    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("로비에 초청 받음");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    //로비 진입시, 내가 로비 참여로 들어올때 동작.
    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        currentLobby = new CSteamID(callback.m_ulSteamIDLobby);
        Debug.Log("현재 참가한 로비 id: " + currentLobby);
        if (NetworkServer.active) { return; }

        string hostAddress = SteamMatchmaking.GetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            HostAddressKey);

        networkManager.networkAddress = hostAddress;

        networkManager.StartClient();
        lobbyIDS.Clear();
    }

    void OnGetLobbiesList(LobbyMatchList_t result)
    {
        string msg = "생성된 로비 탐색 \n";
        string value;
        
        value = _joinCodeRoom ? joinCode_InputText.text : GameKey;

        for (int i = 0; i < result.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
            
            if (SteamMatchmaking.GetLobbyData((CSteamID)lobbyID.m_SteamID, value) == GameValue)
            {
                lobbyIDS.Add(lobbyID);
                SteamMatchmaking.RequestLobbyData(lobbyID);
            }
        }
        
        if (_joinCodeRoom)
        {
            msg = msg + "입장 코드 : " + value + "// 로비 탐색 결과 : " + lobbyIDS.Count;
        }
        else
        {
            if (lobbyIDS.Count == 0)
                MatchManager.instance.CreateNewLobby();

            msg = msg + "타입 : 빠른 매칭 탐색" + "로비 갯수 : " + lobbyIDS.Count;
        }

        Debug.Log(msg);

    }

    void OnGetLobbyInfo(LobbyDataUpdate_t result)
    {
        if (_createLobby || lobbyIDS.Count == 0)
        {
            return;
        }
        inviteCode_Text.text = joinCode_InputText.text;
        MatchManager.instance.AutoJoinLobby(lobbyIDS, result, _joinCodeRoom);
    }

    public void CreateNewLobby(ELobbyType lobbyType, bool isInvite)
    {
        CreateInviteCode();
        inviteRoom = isInvite;
        SteamMatchmaking.CreateLobby(lobbyType, networkManager.maxConnections);
    }


    public void StopMatching()
    {
        LeaveLobby();
        networkManager.StopHost();
        networkManager.StopClient();
    }
}
