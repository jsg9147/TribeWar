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

    [Header("�÷��̾� ���� ����")]
    public bool quickMatch = false;

    public void QuickMatch() => quickMatch = true;
    public void ResetStatus() => quickMatch = false;

    // Start is called before the first frame update
    public override void OnStartServer()
    {
        Debug.Log("���� ������ �����մϴ�");
    }
    public override void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log("Ŭ���̾�Ʈ ���� �Ǿ����ϴ�");
        base.OnClientConnect(conn);
    }
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        Debug.Log("�ùٸ� ���� ������ �ִ��� Ȯ���մϴ�. \n�������� �� : " + SceneManager.GetActiveScene().name.ToString() + ". �ùٸ� �� �̸� : MainMenu");
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            bool isGameLeader = GamePlayers.Count == 0; // isLeader is true if the player count is 0, aka when you are the first player to be added to a server/room

            GamePlayer GamePlayerInstance = Instantiate(gamePlayerPrefab);

            GamePlayerInstance.IsGameLeader = isGameLeader;
            GamePlayerInstance.ConnectionId = conn.connectionId;
            GamePlayerInstance.playerNumber = GamePlayers.Count + 1;

            GamePlayerInstance.playerSteamId = (ulong)SteamMatchmaking.GetLobbyMemberByIndex(SteamLobby.instance.currentLobby, GamePlayers.Count);

            NetworkServer.AddPlayerForConnection(conn, GamePlayerInstance.gameObject);

            Debug.Log(GamePlayerInstance.playerName + "���� �����߽��ϴ�. " + " ���� ���̵�: " + GamePlayerInstance.ConnectionId.ToString() + "\n���� ������ �� : " + GamePlayers.Count);
            if (quickMatch)
            {
                if (GamePlayers.Count == minPlayers)
                {
                    UIManager.instanse.MatchingSuccess();
                    Debug.Log("������ ���� �մϴ�");
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
        // ���ѷε� ���ɼ� ����, false �϶� ��ó �ʿ�
        if (CanStartGame())
        {
            // ������ �� �ε� �Ϸ�Ǹ� �۵�.
            StartCoroutine(GameManager.instance.LoadingComplite());

            // ���� �ƴ� ��ǻ�͵��� �ε� �Ϸ�Ǹ� ����
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

    // �ε� ���� Ȯ���ϴ°�... 
    private bool CanStartGame()
    {
        print("�ε� Ȯ����");

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
