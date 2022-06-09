using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Steamworks;
using Mirror;

public class GamePlayer : NetworkBehaviour
{
    [SerializeField] PlayerProfile playerProfilePrefabs;

    [Header("GamePlayer Info")]
    [SyncVar(hook = nameof(HandlePlayerNameUpdate))] public string playerName;
    [SyncVar] public int ConnectionId;
    [SyncVar] public int playerNumber;
    [SyncVar(hook = nameof(HandlePlayerWinUpdate))] public int playerWin;
    [SyncVar(hook = nameof(HandlePlayerLoseUpdate))] public int playerLose;

    [SerializeField] bool isCreate = false;

    [Header("Game Info")]
    [SyncVar] public bool IsGameLeader = false;
    [SyncVar(hook = nameof(HandlePlayerLoadingStatusChange))] public bool isPlayerLoadingComplite;
    [SyncVar(hook = nameof(HandlePlayerReadyStatusChange))] public bool isPlayerReady;
    [SyncVar(hook = nameof(HandleProfileIcon))] public ulong playerSteamId;
    PlayerProfile profile;

    [Header("Play Info")]
    [SyncVar] public bool playerTurn;
    [SyncVar] public int turnCount;
    [SyncVar(hook = nameof(DeckCountingUpdate))] public int deckCount;

    [Header("bool Synk")]
    [SyncVar] public bool canSelectEffectTarget;
    [SyncVar] public bool canMove;

    CardMove cardMove = new CardMove();

    private MyNetworkManager game;

    private MyNetworkManager Game
    {
        get
        {
            if (game != null)
            {
                return game;
            }
            return game = MyNetworkManager.singleton as MyNetworkManager;
        }
    }

    public override void OnStartAuthority()
    {
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
        CmdSetPlayerRecord((int)WebMain.instance.web.win, (int)WebMain.instance.web.lose);
        gameObject.name = "LocalGamePlayer";
        MatchManager.instance.FindLocalGamePlayer(this);

    }

    [Command]
    private void CmdSetPlayerRecord(int win, int lose)
    {
        this.HandlePlayerWinUpdate(this.playerWin, win);
        this.HandlePlayerLoseUpdate(this.playerLose, lose);

    }

    public void CreatePlayerProfiles()
    {
        if (isCreate)
            return;
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            profile = Instantiate(playerProfilePrefabs);
            profile.PlayerProfileUpdate(playerWin, playerLose, playerName, SteamFriends.GetMediumFriendAvatar(new CSteamID(playerSteamId)), hasAuthority);
            profile.transform.SetParent(Game.playerSlotObject.transform);
            profile.transform.localScale = Vector3.one;
            isCreate = true;
        }
    }

    public void HandleProfileIcon(ulong oldValue, ulong newValue)
    {
        profile?.SetProfileIcon(SteamFriends.GetMediumFriendAvatar(new CSteamID(playerSteamId)));
    }

    public void HandlePlayerWinUpdate(int oldValue, int newValue)
    {
        if (isServer)
            this.playerWin = newValue;
    }

    public void HandlePlayerLoseUpdate(int oldValue, int newValue)
    {
        if (isServer)
            this.playerLose = newValue;

        CreatePlayerProfiles();
    }

    [Command]
    private void CmdSetPlayerName(string playerName)
    {
        this.HandlePlayerNameUpdate(this.playerName, playerName);
    }
    public override void OnStartClient()
    {
        Game.GamePlayers.Add(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void HandlePlayerNameUpdate(string oldValue, string newValue)
    {
        if (isServer)
            this.playerName = newValue;

    }
    public void ChangeLoadingStatus()
    {
        if (hasAuthority)
            CmdChangePlayerLoadingStatus();
    }
    [Command]
    void CmdChangePlayerLoadingStatus()
    {
        this.isPlayerLoadingComplite = !this.isPlayerLoadingComplite;
    }
    void HandlePlayerLoadingStatusChange(bool oldValue, bool newValue)
    {
        if (isServer)
        {
            this.isPlayerLoadingComplite = newValue;
        }
        Game.PlayerAllLoading();
    }

    public void ChangeReadyStatus()
    {
        if (hasAuthority)
            CmdChangePlayerReadyStatus();
    }
    [Command]
    void CmdChangePlayerReadyStatus()
    {
        this.isPlayerReady = !this.isPlayerReady;
    }

    void HandlePlayerReadyStatusChange(bool oldValue, bool newValue)
    {
        if (isServer)
        {
            this.isPlayerReady = newValue;
        }
        profile.ReadyState(this.isPlayerReady);
        Game.PlayerAllReady();
    }

    public void CanEndThisGame()
    {
        if (hasAuthority)
            CanEndGame();
    }

    public void CanEndGame()
    {
        QuitLobby();

        Game.EndGame();
    }


    public void QuitLobby()
    {
        if (hasAuthority)
        {
            if (IsGameLeader)
            {
                Game.StopHost();
            }
            else
            {
                Game.StopClient();
            }
            SteamLobby.instance.LeaveLobby();
        }
    }
    private void OnDestroy()
    {
        if (hasAuthority)
        {
            SteamMatchmaking.LeaveLobby(SteamLobby.instance.currentLobby);
            game = null;
        }
        if (profile != null)
            Destroy(profile.gameObject);
        Debug.Log("LobbyPlayer destroyed. Returning to main menu.");
    }
    public override void OnStopClient()
    {
        Debug.Log(playerName + " is quiting the game.");

        if (hasAuthority == false)
            GameManager.instance?.DisconnectServerPlayer();

        Game.GamePlayers.Remove(this);
    }

    #region In Game Method


    public void DeckCountingUpdate(int oldValue, int newValue)
    {
        if (isServer)
            this.deckCount = newValue;
        CardManager.instance.Deck_Counting_Update(hasAuthority, newValue);
    }

    [Command]
    public void CmdSetDeckCounting(int deckCount)
    {
        DeckCountingUpdate(this.deckCount, deckCount);
    }

    [Command]
    public void CmdSetOutpostPos(Vector3 outpostCoordVec, bool server)
    {
        NetworkRpcFunc.instance.RpcSetOutpost(new Coordinate(outpostCoordVec), server);
    }

    //[Command]
    //public void CmdSummon(bool server, string card_id, Vector3 coordVec)
    //{
    //    NetworkRpcFunc.instance.RpcSummon(server, card_id, new Coordinate(coordVec));
    //    NetworkRpcFunc.instance.RpcSetMostFrontOrderInit(server);
    //}

    //[Command]
    //public void CmdSetSummonCoord(Vector3 coordVec)
    //{
    //    NetworkRpcFunc.instance.RpcSetCoordinateData(coordVec);
    //}

    [Command]
    public void CmdTryPutCard(bool server, string card_id, Vector3 selectPos)
    {
        NetworkRpcFunc.instance.RpcTryPutCard(server, card_id, selectPos);
    }

    [Command]
    public void CmdSelectTribute(int entityID, bool server)
    {
        NetworkRpcFunc.instance.RpcSelectTribute(server, entityID);
    }

    [Command]
    public void CmdEffectSolve(string card_id, bool server)
    {
        NetworkRpcFunc.instance.RpcEffectSolve(card_id, server);
    }


    [Command]
    public void CmdAttack(int attackerID, int defenderID, bool server)
    {
        NetworkRpcFunc.instance.RpcAttack(attackerID, defenderID, server);
    }

    [Command]
    public void CmdOutpostAttack(int attackerID, Vector3 outpostVector, bool server)
    {
        NetworkRpcFunc.instance.RpcOutpostAttack(attackerID, new Coordinate(outpostVector), server);
    }

    [Command]
    public void CmdReloadHandCard(bool server)
    {
        NetworkRpcFunc.instance.RpcReloadCard(server);
    }

    [Command]
    public void CmdSelectEffectTarget(int entityID, bool targetPlayer, bool server)
    {
        NetworkRpcFunc.instance.RpcSelect_Effect_Target(entityID, targetPlayer, server);
    }

    [Command]
    public void CmdRandomTargetAppoint(int entity_Id, string card_id)
    {
        //List<Entity> playerEntities = EntityManager.instance.playerEntities;
        //List<Entity> opponentEntities = EntityManager.instance.opponentEntities;

        //bool randomPlayer;

        //if (playerEntities.Count == 0 && opponentEntities.Count == 0)
        //    return;
        //else if (playerEntities.Count == 0)
        //    randomPlayer = false;
        //else if (opponentEntities.Count == 0)
        //    randomPlayer = true;
        //else
        //    randomPlayer = Random.Range(0, 2) == 0;

        //int targetIndex =
        //    randomPlayer ? Random.Range(0, playerEntities.Count - 1) : Random.Range(0, opponentEntities.Count - 1);

        NetworkRpcFunc.instance.RpcRandomTargetEffect(entity_Id, card_id);
    }

    [Command]
    public void CmdCardMove(int entityID, bool targetPlyaer, Vector3 movePos, bool server)
    {
        NetworkRpcFunc.instance.RpcCardMove(entityID, targetPlyaer, movePos, server);
    }


    [Command]
    public void CmdTurnSetup()
    {
        NetworkRpcFunc.instance.RpcStartCardDealing();
        NetworkRpcFunc.instance.RpcTurnSetup(Random.Range(0, 2));
    }

    [Command]
    public void CmdTurnEnd()
    {
        NetworkRpcFunc.instance.RpcTurnEnd();
    }

    [Command]
    public void CmdGameResult(bool gameResult, bool server)
    {
        NetworkRpcFunc.instance.RpcGameResult(gameResult, server);
    }

    [Command]
    public void CmdMoveEffect(int entity_Id, Vector3 tilePos)
    {
        NetworkRpcFunc.instance.RpcTarget_Effect_Solver(entity_Id, tilePos);
    }


    #endregion
}
