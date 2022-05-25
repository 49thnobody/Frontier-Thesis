using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public OutputController Authority;

    public Transform HandLayout;
    public CardController CardPrefab;
    private List<CardController> _hand;
    private List<Card> _handCards;
    public List<Card> Hand => _handCards;

    public Transform BaseLayout;
    public List<CardController> Bases;

    private List<Card> _deck;
    private List<Card> _discardPile;
    public List<Card> DiscardPile => _discardPile;

    private void Awake()
    {
        instance = this;
        _discardPile = new List<Card>();
        _hand = new List<CardController>();
        _handCards = new List<Card>();
        _deck = new List<Card>();
        Bases = new List<CardController>();
    }

    private void Start()
    {
        GameManager.instance.OnGameStart += GameStart;

        GameManager.instance.OnTestDraw += TestDraw;

        CardSystem.instance.OnScrap += OnScrap;
    }

    public void TakeDamage(int damage)
    {
        Authority.UpdateValue(Authority.Value - damage);
        if (Authority.Value <= 0)
            ;//lose
    }

    private void OnScrap(Card card)
    {
        if (_discardPile.Contains(card))
            _discardPile.Remove(card);
        if (_hand.ConvertAll(p => p.Card).Contains(card))
        {
            var cardC = _hand.Find(p => p.Card == card);
            _hand.Remove(cardC);
            Destroy(cardC.gameObject);
        }
    }

    private void TestDraw()
    {
        Reset();

        for (int i = 0; i < 10; i++)
        {
            _deck.Add(new Card("Burrower", 3, Faction.Slugs,
                new List<Effect>() { new Effect(EffectGroup.Priamry, EffectType.Draw, 1) }));
        }

        for (int i = 0; i < 5; i++)
        {
            DrawCard();
        }
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

    public void StartTurn()
    {
        // discard cards
    }

    public void EndTurn()
    {
        _discardPile.AddRange(_handCards);
        _handCards.Clear();

        for (int i = _hand.Count - 1; i >= 0; i--)
        {
            Destroy(_hand[i].gameObject);
            _hand.RemoveAt(i);
        }

        for (int i = 0; i < 5; i++)
        {
            DrawCard();
        }
    }

    private void Reset()
    {
        Authority.UpdateValue(50);

        for (int i = _hand.Count - 1; i >= 0; i--)
        {
            Destroy(_hand[i].gameObject);
            _hand.RemoveAt(i);
        }

        for (int i = Bases.Count - 1; i >= 0; i--)
        {
            Destroy(Bases[i]);
            Bases.RemoveAt(i);
        }

        _deck.Clear();
    }

    public void DrawCard()
    {
        if (_deck.Count == 0)
            Shuffle();

        CardController card = Instantiate(CardPrefab, HandLayout);
        card.Set(_deck[_deck.Count - 1]);
        _handCards.Add(_deck[_deck.Count - 1]);
        _hand.Add(card);
        _deck.RemoveAt(_deck.Count - 1);
    }

    private int _discardCount;
    public void DiscardOnStart()
    {
        _discardCount++;
    }

    public void Discard(List<Card> cards)
    {
        foreach (var card in cards)
        {
            var cardC = _hand.Find(p => p.Card == card);
            if (cardC != null)
            {
                _hand.Remove(cardC);
                Destroy(cardC.gameObject);
            }
            _handCards.Remove(card);
            _discardPile.Add(card);
        }
    }

    public void DiscardBase(CardController basement)
    {
        Bases.Remove(basement);
        Destroy(basement.gameObject);
    }

    public void OnBuy(Card card)
    {
        _discardPile.Add(card);
        // update ui
    }

    private void Shuffle()
    {
        var allCards = new List<Card>();
        allCards.AddRange(_deck);
        allCards.AddRange(_discardPile);
        _deck.Clear();
        _discardPile.Clear();

        int cardCount = allCards.Count;

        while (_deck.Count < cardCount)
        {
            int currentIndex = Random.Range(0, allCards.Count);

            _deck.Add(allCards[currentIndex]);
            allCards.RemoveAt(currentIndex);
        }

        _deck = allCards;
    }

    public void PlaceBase(CardController card)
    {
        Bases.Add(card);
        card.Parent = BaseLayout;
    }
}
