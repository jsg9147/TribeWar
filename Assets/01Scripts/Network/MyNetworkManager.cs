using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;
using UnityEngine.SceneManagement;

public class MyNetworkManager : NetworkManager
{
    [SerializeField] private GamePlayer gamePlayerPrefab;
    [SerializeField] public int minPlayers;

    public List<GamePlayer> GamePlayers { get; } = new List<GamePlayer>();

    public bool quickMatch = false;

    public void QuickMatch() => quickMatch = true;
    public void ResetStatus() => quickMatch = false;

    // Start is called before the first frame update
    public override void OnStartServer()
    {
        Debug.Log("Start mirror server");
    }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Debug.Log("Player is add on server : " + SceneManager.GetActiveScene().name.ToString() + "proper scene name : MainMenu");
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            bool isGameLeader = GamePlayers.Count == 0; // isLeader is true if the player count is 0, aka when you are the first player to be added to a server/room

            GamePlayer GamePlayerInstance = Instantiate(gamePlayerPrefab);

            GamePlayerInstance.IsGameLeader = isGameLeader;
            GamePlayerInstance.ConnectionId = conn.connectionId;
            GamePlayerInstance.playerNumber = GamePlayers.Count + 1;

            GamePlayerInstance.playerSteamId = (ulong)SteamMatchmaking.GetLobbyMemberByIndex(SteamLobby.instance.currentLobby, GamePlayers.Count);

            NetworkServer.AddPlayerForConnection(conn, GamePlayerInstance.gameObject);

            Debug.Log(GamePlayerInstance.playerName + " is connected. " + " player connection ID : " + GamePlayerInstance.ConnectionId.ToString() + "\nCorrent player count : " + GamePlayers.Count);
            if (quickMatch)
            {
                if (GamePlayers.Count == minPlayers)
                {

                    Debug.Log("Let's start the game");
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
        if (CanStartGame())
        {
            if (NetworkRpcFunc.instance != null)
                NetworkRpcFunc.instance.RpcLoadingComplite();
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


    [ContextMenu("Start Game")]
    public void StartGame()
    {
        StartCoroutine(GameStart());
    }

    IEnumerator GameStart()
    {
        yield return new WaitForSeconds(1f);
        
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            ServerChangeScene("NetPlay");
        }
    }

    private bool CanStartGame()
    {
        if (numPlayers < minPlayers)
            return false;

        Debug.Log("Check the game Can Started");

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
        if (SceneManager.GetActiveScene().name == "NetPlay")
        {
            StopClient();
            StopHost();

            ServerChangeScene("MainMenu");
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if (conn.identity != null)
        {
            GamePlayer player = conn.identity.GetComponent<GamePlayer>();

            if (player.playerSteamId != SteamUser.GetSteamID().m_SteamID)
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
