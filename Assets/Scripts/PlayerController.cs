using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public OutputController Authority;

    public Transform Hand;
    public CardController CardPrefab;
    private List<CardController> _hand;

    public Transform BaseLayout;
    public List<CardController> Bases;

    private List<Card> _deck;
    private List<Card> _discardPile;

    private void Awake()
    {
        instance = this;
        _discardPile = new List<Card>();
        _hand = new List<CardController>();
        Bases = new List<CardController>();
    }

    private void Start()
    {
        GameManager.instance.OnGameStart += GameStart;
    }

    public void GameStart()
    {
        Authority.UpdateValue(50);
        _deck = CardSystem.instance.StartingDeck;
        Shuffle();

        for (int i = 0; i < 5; i++)
        {
            DrawCard();
        }
    }

    public void DrawCard()
    {
        CardController card = Instantiate(CardPrefab, Hand);
        card.Set(_deck[_deck.Count - 1]);
        _hand.Add(card);
        _deck.RemoveAt(_deck.Count - 1);

        if (_deck.Count == 0)
            Shuffle();
    }

    private void Shuffle()
    {
        var allCards = new List<Card>();
        allCards.AddRange(_deck);
        allCards.AddRange(_discardPile);

        while (_deck.Count < allCards.Count)
        {
            int currentIndex = Random.Range(0, allCards.Count);

            _deck.Add(allCards[currentIndex]);
            allCards.RemoveAt(currentIndex);
        }

        _deck = allCards;
        _discardPile.Clear();
    }

    public void PlaceBase(CardController card)
    {
        Bases.Add(card);
        card.Parent = BaseLayout;
    }
}
