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
        _deck = new List<Card>();   
        Bases = new List<CardController>();
    }

    private void Start()
    {
        GameManager.instance.OnGameStart += GameStart;

        GameManager.instance.OnTestDraw += TestDraw;
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

        CardController card = Instantiate(CardPrefab, Hand);
        card.Set(_deck[_deck.Count - 1]);
        _hand.Add(card);
        _deck.RemoveAt(_deck.Count - 1);
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
