using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public static EnemyController instance;


    public Transform BaseLayout;
    public List<CardController> Bases;

    public OutputController Authority;

    public CardController CardPrefab;
    private List<Card> _handCards;

    private List<Card> _deck;
    private List<Card> _discardPile;
    public List<Card> DiscardPile => _discardPile;

    private void Awake()
    {
        instance = this;
        _discardPile = new List<Card>();
        _handCards = new List<Card>();
        _deck = new List<Card>();
        Bases = new List<CardController>();
    }

    private void Start()
    {
        GameManager.instance.OnGameStart += GameStart;
    }

    public void EndTurn()
    {
        _discardPile.AddRange(_handCards);
        _handCards.Clear();

        for (int i = 0; i < 5; i++)
        {
            DrawCard();
        }
    }

    public void DrawCard()
    {
        if (_deck.Count == 0)
            Shuffle();

        _handCards.Add(_deck[_deck.Count - 1]);
        _deck.RemoveAt(_deck.Count - 1);
    }

    private void Discard(Card card)
    {
        if (_handCards.Contains(card))
        {
            _handCards.Remove(card);
            _discardPile.Add(card);
        }
    }

    public void PlaceBase(CardController card)
    {
        Bases.Add(card);
        card.Parent = BaseLayout;
    }

    public void DiscardBase(CardController basement)
    {
        Bases.Remove(basement);
        Destroy(basement.gameObject);
    }

    public void TakeDamage(int damage)
    {
        Authority.UpdateValue(Authority.Value - damage);
        if (Authority.Value <= 0)
            ;//lose
    }

    private void Shuffle()
    {
        var allCards = new List<Card>();
        allCards.AddRange(_deck);
        allCards.AddRange(_discardPile);
        _discardPile.Clear();
        _deck.AddRange(allCards);
        CardSystem.instance.Shuffle(ref _deck);
    }

    public void OnMyTurn()
    {
        while (_handCards.Count>0)
        {
            PlayAreaController.instance.PlaceEnemyCard(_handCards[_handCards.Count - 1]);
            Discard(_handCards[_handCards.Count - 1]);
        }

        PlayAreaController.instance.EndTurn();
    }

    public void GameStart()
    {
        Reset();
        _deck.AddRange(CardSystem.instance.StartingDeck);

        for (int i = 0; i < 5; i++)
        {
            DrawCard();
        }
    }

    private void Reset()
    {
        Authority.UpdateValue(50);

        _handCards.Clear();
        _discardPile.Clear();

        for (int i = Bases.Count - 1; i >= 0; i--)
        {
            Destroy(Bases[i]);
            Bases.RemoveAt(i);
        }

        _deck.Clear();
    }

    internal void DiscardCard()
    {
        var cards = new List<Card>();
        cards.AddRange(_deck);
        cards.Sort(delegate (Card card1, Card card2)
        {
            return card1.Cost.CompareTo(card2.Cost);
        });

        int i = 0;
        var cardsToDiscard = new List<Card>();
        do
        {
            cardsToDiscard.Add(cards[i]);
            i++;
        } while (cards[i].Cost == cards[i - 1].Cost);

        int index = UnityEngine.Random.Range(0, cardsToDiscard.Count);

        Discard(cardsToDiscard[index]);
    }
}
