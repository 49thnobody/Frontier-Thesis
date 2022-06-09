using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public static MenuController instance;
    private void Awake()
    {
        instance = this;
    }

    [SerializeField] private Button _buttonStartGame;
    [SerializeField] private Button _buttonRules;
    [SerializeField] private Button _buttonExit;

    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private GameObject _rulePanel;
    [SerializeField] private Button _buttonCloseRule;

    private void Start()
    {
        _buttonStartGame.onClick.AddListener(StartGame);
        _buttonRules.onClick.AddListener(ShowRules);
        _buttonExit.onClick.AddListener(Exit);
        _buttonCloseRule.onClick.AddListener(HideRules);
    }

    private void StartGame()
    {
        _menuPanel.SetActive(false);
        GameManager.instance.Play();
    }

    private void ShowRules()
    {
        _rulePanel.SetActive(true);
    }

    private void HideRules()
    {
        _rulePanel.SetActive(false);
    }

    private void Exit()
    {
        Application.Quit();
    }

    public void Show()
    {
        _menuPanel.SetActive(true);
    }
}
