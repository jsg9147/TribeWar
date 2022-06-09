using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Mirror;
using UnityEngine.SceneManagement;

public class SingleManager : MonoBehaviour
{
    public static SingleManager instance
    {
        get; private set;
    }

    [SerializeField] GameObject LoadingPanel;
    [SerializeField] NotificationPanel notificationPanel;

    [SerializeField] ResultPanel resultPanel;
    [SerializeField] GameObject endTurnBtn;

    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject menuWindow;
    [SerializeField] GameObject optionWindow;

    [SerializeField] SingleCameraEffect cameraEffect;

    WaitForSeconds delay2 = new WaitForSeconds(2);

    bool gameEnded;
    public bool clickBlock;
    public bool isTutorial = true;

    public bool isStarted;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Init();
    }

    void Init()
    {
        clickBlock = true;
        gameEnded = false;
        isStarted = false;
        UISetup();

        if (!isTutorial)
        {
            LoadingComplited();
            LoadingPanel.SetActive(true);
        }
    }

    public void UISetup()
    {
        notificationPanel.ScaleZero();
        resultPanel.ScaleZero();
        cameraEffect.SetGrayScale(false);
        menuPanel.SetActive(false);
    }

    public IEnumerator LoadingComplite()
    {
        Sequence sequence = DOTween.Sequence()
        .Append(LoadingPanel.transform.DOScale(Vector3.zero, 1.5f)).SetEase(Ease.InCubic);
        yield return new WaitForSeconds(0.5f);
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
            SingleTurnManager.OnAddCard?.Invoke(true);

        if (Input.GetKeyDown(KeyCode.Keypad2))
            SingleTurnManager.OnAddCard?.Invoke(false);

        if (Input.GetKeyDown(KeyCode.Keypad3))
            SingleTurnManager.instance.TurnEnd();

        if (Input.GetKeyDown(KeyCode.Keypad7))
            StartGame();

    }

    public void StartGame()
    {
        SingleCardManager.instance.StartCardDealing();
        SingleTurnManager.instance.TurnSetup(Random.Range(0, 1));
        isStarted = true;
    }


    public void Notification(string msg) => notificationPanel.Show(msg);

    public void GameResult(bool gameResult)
    {
        if (gameEnded == false)
        {
            StartCoroutine(Gameover(gameResult));
        }
        gameEnded = true;
    }

    IEnumerator Gameover(bool gameResult)
    {
        yield return new WaitForSeconds(1f);

        SingleTurnManager.instance.isLoading = true;
        endTurnBtn.SetActive(false);

        SingleTurnManager.instance.isLoading = true;
        resultPanel.Show(gameResult ? "승리" : "패배");
        cameraEffect.SetGrayScale(!gameResult);
        yield return delay2;

        SceneManager.LoadScene("MainMenu");
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

            if (menuPanel.activeSelf)
            {
                optionWindow.SetActive(false);
                menuWindow.SetActive(true);
            }

            clickBlock = menuPanel.activeSelf;

            foreach (var tile in MapManager.instance.mapData)
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
