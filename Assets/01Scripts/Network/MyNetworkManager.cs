using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;
using UnityEngine.SceneManagement;

public class MyNetworkManager : NetworkManager
{
    [SerializeField] private GamePlayer gamePlayerPrefab;
    [SerializeField] public GameObject playerSlotObject;
    [SerializeField] public int _minPlayers;
    int minPlayers
    {
        get { return _minPlayers - 1; }
        set { _minPlayers = value + 1; }
    }
    public List<GamePlayer> GamePlayers { get; } = new List<GamePlayer>();

    [Header("플레이어 상태 변수")]
    public bool quickMatch = false;

    public void QuickMatch() => quickMatch = true;
    public void ResetStatus() => quickMatch = false;

    // Start is called before the first frame update
    public override void OnStartServer()
    {
        Debug.Log("서버 연결을 시작합니다");
    }
    public override void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log("클라이언트 접속 되었습니다");
        base.OnClientConnect(conn);
    }
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        Debug.Log("올바른 씬에 접속해 있는지 확인합니다. \n접속자의 씬 : " + SceneManager.GetActiveScene().name.ToString() + ". 올바른 씬 이름 : MainMenu");
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            bool isGameLeader = GamePlayers.Count == 0; // isLeader is true if the player count is 0, aka when you are the first player to be added to a server/room

            GamePlayer GamePlayerInstance = Instantiate(gamePlayerPrefab);

            GamePlayerInstance.IsGameLeader = isGameLeader;
            GamePlayerInstance.ConnectionId = conn.connectionId;
            GamePlayerInstance.playerNumber = GamePlayers.Count + 1;

            GamePlayerInstance.playerSteamId = (ulong)SteamMatchmaking.GetLobbyMemberByIndex(SteamLobby.instance.currentLobby, GamePlayers.Count);

            NetworkServer.AddPlayerForConnection(conn, GamePlayerInstance.gameObject);

            Debug.Log(GamePlayerInstance.playerName + "님이 접속했습니다. " + " 접속 아이디: " + GamePlayerInstance.ConnectionId.ToString() + "\n현재 접속자 수 : " + GamePlayers.Count);
            if (quickMatch)
            {
                if (GamePlayers.Count == minPlayers)
                {
                    UIManager.instanse.MatchingSuccess();
                    Debug.Log("게임을 시작 합니다");
                    StartGame();
                }
            }
        }
    }

    public void PlayerReady()
    {
        GamePlayer player = GamePlayers.Find(x => x.gameObject.name == "LocalGamePlayer");
        player.ChangeReadyStatus();
    }

    public void PlayerAllLoading()
    {
        // 무한로딩 가능성 있음, false 일때 대처 필요
        if (CanStartGame())
        {
            // 방장이 맵 로딩 완료되면 작동.
            StartCoroutine(GameManager.instance.LoadingComplite());

            // 방장 아닌 컴퓨터들이 로딩 완료되면 동작
            NetworkRpcFunc.instance?.RpcLoadingComplite();
        }
    }

    public void PlayerAllReady()
    {
        if (AllPlayerReady() && GamePlayers.Count >= minPlayers)
        {
            StartGame();
        }
    }

    public void ExitRoom()
    {
        StopClient();
        StopHost();
    }


    [ContextMenu ("Start Game")]
    public void StartGame()
    {
        StartCoroutine(GameStart());
    }

    IEnumerator GameStart()
    {
        yield return new WaitForSeconds(3f);
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            ServerChangeScene("Scene_GamePlay");
        }
    }

    // 로딩 여부 확인하는것... 
    private bool CanStartGame()
    {
        print("로딩 확인중");

        if (numPlayers < minPlayers)
            return false;

        foreach (GamePlayer player in GamePlayers)
        {
            if (!player.isPlayerLoadingComplite)
                return false;
        }
        return true;
    }

    private bool AllPlayerReady()
    {
        if (numPlayers < minPlayers)
            return false;

        foreach (GamePlayer player in GamePlayers)
        {
            if (!player.isPlayerReady)
                return false;
        }
        return true;
    }

    public void EndGame()
    {
        if (SceneManager.GetActiveScene().name == "Scene_GamePlay")
        {
            StopClient();
            StopHost();

            ServerChangeScene("MainMenu");
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (conn.identity != null)
        {
            GamePlayer player = conn.identity.GetComponent<GamePlayer>();

            if(player.playerSteamId != SteamUser.GetSteamID().m_SteamID)
            {
                if (GameManager.instance != null)
                    GameManager.instance.GameResult(true);
            }

            GamePlayers.Remove(player);
        }
        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        GamePlayers.Clear();
    }
}
