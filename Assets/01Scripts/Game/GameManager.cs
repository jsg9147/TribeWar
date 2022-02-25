using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Mirror;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [Header("판넬 관리")]
    [SerializeField] GameObject LoadingPanel;
    [SerializeField] NotificationPanel notificationPanel;
    [SerializeField] ConfirmationPanel confirmationPanel;
    [SerializeField] ResultPanel resultPanel;
    [SerializeField] GameObject endTurnBtn;

    [Header("메뉴창 관리")]
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject menuWindow;
    [SerializeField] GameObject optionWindow;

    [SerializeField] CameraEffect cameraEffect;

    [Header("P2P용 오브젝트 and 스크립트")]
    public GameObject localGamePlayerObject;
    public GamePlayer localGamePlayerScript;

    WaitForSeconds delay2 = new WaitForSeconds(2);

    bool gameEnded;
    public bool clickBlock;

    private void Awake()
    {
        UISetup();
        clickBlock = true;
        instance = this;
    }

    private void Start()
    {   
        FindLocalGamePlayer();
        gameEnded = false;
    }

    public void UISetup()
    {
        notificationPanel.ScaleZero();
        resultPanel.ScaleZero();
        cameraEffect.SetGrayScale(false);
        LoadingPanel.SetActive(true);
        menuPanel.SetActive(false);
    }

    public IEnumerator LoadingComplite()
    {
        Sequence sequence = DOTween.Sequence()
        .Append(LoadingPanel.transform.DOScale(Vector3.zero, 1.5f)).SetEase(Ease.InCubic);
        yield return new WaitForSeconds(1f);
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
        localGamePlayerObject = GameObject.Find("LocalGamePlayer");
        localGamePlayerScript = localGamePlayerObject.GetComponent<GamePlayer>();
    }


    public void StartGame()
    {
        localGamePlayerScript.CmdTurnSetup();
    }


    public void Notification(string msg) => notificationPanel.Show(msg);
    public void Confirmation(string msg) => confirmationPanel.Show(msg);
    public void DisapearConfirmation() => confirmationPanel.ScaleZero();
    
    public void GameResult(bool gameResult, bool server)
    {
        bool isMine = NetworkRpcFunc.instance.isServer == server;

        gameResult = isMine ? gameResult : !gameResult;

        if (gameEnded == false)
        {
            StartCoroutine(Gameover(gameResult));
        }
        gameEnded = true;
    }

    public void GameResult(bool gameResult)
    {
        if(NetworkRpcFunc.instance.isClient)
            localGamePlayerScript.CmdGameResult(gameResult, NetworkRpcFunc.instance.isServer);
        if (NetworkRpcFunc.instance.isServer)
            NetworkRpcFunc.instance.RpcGameResult(gameResult, NetworkRpcFunc.instance.isServer);

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

        TurnManager.instance.isLoading = true;
        endTurnBtn.SetActive(false);

        TurnManager.instance.isLoading = true;
        resultPanel.Show(gameResult ? "승리" : "패배");
        cameraEffect.SetGrayScale(!gameResult);
        yield return delay2;

        localGamePlayerScript.CanEndGame();
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
        GameResult(false);
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

            if(menuPanel.activeSelf)
            {
                optionWindow.SetActive(false);
                menuWindow.SetActive(true);
            }

            clickBlock = menuPanel.activeSelf;

            foreach(var tile in MapManager.instance.mapData)
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
