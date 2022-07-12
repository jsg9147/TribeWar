using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Mirror;
using UnityEngine.SceneManagement;
using DarkTonic.MasterAudio;

public class GameManager : MonoBehaviour
{
    public static GameManager instance
    {
        get; private set;
    }

    public bool MultiMode;

    [SerializeField] ProfileController profileController;
    [SerializeField] GameObject LoadingPanel;
    [SerializeField] GameObject PlayerInfoPanel;
    [SerializeField] GameObject EnermyInfoPanel;
    [SerializeField] NotificationPanel notificationPanel;

    [SerializeField] ResultPanel resultPanel;
    [SerializeField] GameObject endTurnBtn;

    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject menuWindow;
    [SerializeField] GameObject optionWindow;

    [SerializeField] CameraEffect cameraEffect;

    public GameObject localGamePlayerObject;
    public GamePlayer localGamePlayerScript;

    WaitForSeconds delay2 = new WaitForSeconds(2);

    bool gameEnded;
    public bool clickBlock;

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

    private void Awake()
    {
        Init();
        clickBlock = true;
        instance = this;
    }

    private void Start()
    {
        FindLocalGamePlayer();
        gameEnded = false;
    }

    public void Init()
    {
        notificationPanel.ScaleZero();
        resultPanel.ScaleZero();
        cameraEffect.SetGrayScale(false);
        menuPanel.SetActive(false);
        if(MultiMode)
            LoadingPanel.SetActive(true);
    }

    public bool IsMine(bool isServer)
    {
        bool mine;
        if (MultiMode)
        {
            mine = NetworkRpcFunc.instance.isServer == isServer;
        }
        else
        {
            mine = isServer;
        }
        return mine;
    }

    // 방장 두번실행되는 문제 있음, 트러블은 안나지만 고쳐야함
    public IEnumerator LoadingComplite()
    {
        MasterAudio.PlaySound("PlayStart");

        // 추후 컴퓨터 기록도 넣을까 고민중
        if (MultiMode)
        {

            foreach (GamePlayer player in Game.GamePlayers)
            {
                if (player.playerSteamId == Steamworks.SteamUser.GetSteamID().m_SteamID)
                {
                    profileController.SetMyProfile(player);
                }
                else
                {
                    profileController.SetOtherProfile(player);
                }
            }
            profileController.BlackPanelSetActive(false);
            yield return new WaitForSeconds(5f);

            Sequence sequence = DOTween.Sequence()
            .Append(PlayerInfoPanel.transform.DOMoveX(-960, 1.5f)).SetEase(Ease.InCubic);

            Sequence sequence1 = DOTween.Sequence()
            .Append(EnermyInfoPanel.transform.DOMoveX(960, 1.5f)).SetEase(Ease.InCubic);
        }

        Notification("본진을 \n설치 하세요");
        clickBlock = false;
    }

    public void LoadingComplited()
    {
        StartCoroutine(LoadingComplite());
    }

    void Update()
    {
#if UNITY_EDITOR
        InputCheatKey();
#endif

        MenuPanelIsOn();
    }

    private void OnApplicationQuit()
    {
        // 기록 저장용 함수 필요
        GameResult(false);
    }

    void InputCheatKey()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
            TurnManager.OnAddCard?.Invoke(true);

        if (Input.GetKeyDown(KeyCode.Keypad2))
            TurnManager.OnAddCard?.Invoke(false);

        if (Input.GetKeyDown(KeyCode.Keypad3))
            TurnManager.instance.StartTurn();

        if (Input.GetKeyDown(KeyCode.Keypad7))
            StartGame();

    }

    public void FindLocalGamePlayer()
    {
        if (MultiMode)
        {
            localGamePlayerObject = GameObject.Find("LocalGamePlayer");
            localGamePlayerScript = localGamePlayerObject.GetComponent<GamePlayer>();
        }
    }


    public void StartGame()
    {
        if (MultiMode)
        {
            localGamePlayerScript.CmdTurnSetup();
        }
        else
        {
            TurnManager.instance.TurnSetup(Random.Range(0, 2));
        }
    }

    public void EndTurnBtnSetup(bool isActive) => endTurnBtn.transform.GetComponent<EndTurnBtn>().Setup(isActive);
    public void Notification(string msg) => notificationPanel.Show(msg);

    public void GameResult(bool gameResult, bool server)
    {
        bool isMine;
        if (MultiMode)
        {
            isMine = NetworkRpcFunc.instance.isServer == server;
        }
        else
        {
            isMine = server;
        }

        gameResult = isMine ? !gameResult : gameResult;

        if (gameEnded == false)
        {
            StartCoroutine(Gameover(gameResult));
        }
        gameEnded = true;
    }

    public void GameResult(bool gameResult)
    {
        if (MultiMode)
        {
            if (NetworkRpcFunc.instance.isClient)
                localGamePlayerScript.CmdGameResult(gameResult, NetworkRpcFunc.instance.isServer);
            if (NetworkRpcFunc.instance.isServer)
                NetworkRpcFunc.instance.RpcGameResult(gameResult, NetworkRpcFunc.instance.isServer);
        }

        if (gameEnded == false)
        {
            StartCoroutine(Gameover(gameResult));
        }
        gameEnded = true;
    }

    public void DisconnectServerPlayer()
    {
        if (gameEnded == false)
        {
            StartCoroutine(DisconnectGameover());
        }
        gameEnded = true;
    }

    IEnumerator Gameover(bool gameResult)
    {
        yield return new WaitForSeconds(1f);
        if (MultiMode == false)
        {
            gameResult = !gameResult;
        }

        CardManager.instance.Can_Use_Effect(false);

        TurnManager.instance.isLoading = true;
        endTurnBtn.SetActive(false);

        TurnManager.instance.isLoading = true;
        resultPanel.Show(gameResult ? "승리" : "패배");
        cameraEffect.SetGrayScale(!gameResult);
        yield return delay2;

        if (MultiMode)
        {
            localGamePlayerScript.CanEndGame();
        }
        else
        {
            SceneManager.LoadSceneAsync("MainMenu");
        }
    }

    IEnumerator DisconnectGameover()
    {
        TurnManager.instance.isLoading = true;
        resultPanel.Show("승리");
        cameraEffect.SetGrayScale(false);
        yield return new WaitForSeconds(1f);
        NetworkManager.loadingSceneAsync = SceneManager.LoadSceneAsync("MainMenu");
    }

    public void Surrender()
    {
        if (AIManager.instance.SinglePlay)
        {
            GameResult(true);
        }
        else
        {
            GameResult(false);
        }
        menuPanel.SetActive(false);
    }

    public void GameQuit()
    {
        GameResult(false);
        Application.Quit();
    }

    void MenuPanelIsOn()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && gameEnded == false)
        {
            menuPanel.SetActive(!menuPanel.activeSelf);

            if (menuPanel.activeSelf)
            {
                optionWindow.SetActive(false);
                menuWindow.SetActive(true);
            }

            clickBlock = menuPanel.activeSelf;

            foreach (Tile tile in MapManager.instance.mapData)
            {
                tile.ResetColor();
            }

            CardManager.instance?.MouseEventInit();
            EntityManager.instance?.MouseEventInit();
        }
    }

    public void OptionWindowActive()
    {
        if (menuWindow.activeSelf)
            menuWindow.SetActive(false);

        optionWindow.SetActive(true);
    }
}
