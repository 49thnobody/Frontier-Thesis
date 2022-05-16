using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;

    private void Awake()
    {
        instance = this;
    }
    
    public Turn Turn { get; private set; }
}
