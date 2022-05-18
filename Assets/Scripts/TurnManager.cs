using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;

    private void Awake()
    {
        instance = this;

        Turn = Turn.PlayerTurn;
    }
    
    public Turn Turn { get; private set; }
}
