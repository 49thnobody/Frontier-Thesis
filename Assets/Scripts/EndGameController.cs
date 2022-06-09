using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndGameController : MonoBehaviour
{
    public static EndGameController instance;
    private void Awake()
    {
        instance = this;
    }

    [SerializeField] private GameObject _endGamePanel;

    [SerializeField] private Button _buttonMenu;
    [SerializeField] private Button _buttonExit;

    [SerializeField] private TextMeshProUGUI _winLoseText;

    private void Start()
    {
        _buttonMenu.onClick.AddListener(ToMenu);
        _buttonExit.onClick.AddListener(Exit);
    }

    private void ToMenu()
    {
        MenuController.instance.Show();
    }

    private void Exit()
    {
        Application.Quit();
    }

    public void Show(bool isWin)
    {
        _endGamePanel.SetActive(true);
        if (isWin) _winLoseText.text = "Победа!";
        else _winLoseText.text = "Поражение";
    }
}
