using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    public delegate void GameStart();
    public event GameStart OnGameStart;

    public delegate void TestDraw();
    public event TestDraw OnTestDraw;

    public void Play()
    {
        OnGameStart?.Invoke();
    }

    public void TestingDraw()
    {
        OnTestDraw?.Invoke();
    }
}
