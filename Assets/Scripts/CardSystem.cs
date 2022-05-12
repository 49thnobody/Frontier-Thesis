using System.Collections.Generic;
using UnityEngine;

public class CardSystem : MonoBehaviour
{
    public static CardSystem instance;

    private void Awake()
    {
        instance = this;
    }

    public List<Card> StartingDeck { get; private set; }
    private List<Card> _tradeDeck;
    private List<CardController> _tradeRow;
}
