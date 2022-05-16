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

    public void Play()
    {
        OnGameStart?.Invoke();
    }
}
