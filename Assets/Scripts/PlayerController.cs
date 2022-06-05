using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [SerializeField] private OutputController Authority;

    [SerializeField] private Transform HandLayout;
    [SerializeField] private CardController CardPrefab;
    private List<CardController> _hand;
    private List<Card> _handCards;
    public List<Card> Hand => _handCards;

    [SerializeField] private Transform BaseLayout;
    public List<CardController> Bases;

    [SerializeField] private TextMeshProUGUI _deckCardCount;
    private List<Card> _deck;
    private List<Card> _discardPile;
    public List<Card> DiscardPile => _discardPile;

    [SerializeField] private Transform _discardPileTransform;

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

    public void GameStart()
    {
        Reset();
        _deck.AddRange(CardSystem.instance.StartingDeck);
        Shuffle();

        for (int i = 0; i < 5; i++)
        {
            DrawCard();
        }
    }

    public void EndTurn()
    {
        _discardCount = 0;
        _discardPile.AddRange(_handCards);
        _handCards.Clear();

        for (int i = _hand.Count - 1; i >= 0; i--)
        {
            Destroy(_hand[i].gameObject);
            _hand.RemoveAt(i);
        }
        UpdateDiscardImage();

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
        _handCards.Clear();

        for (int i = Bases.Count - 1; i >= 0; i--)
        {
            Destroy(Bases[i]);
            Bases.RemoveAt(i);
        }

        _deck.Clear();
        _discardPile.Clear();
        UpdateDiscardImage();
    }

    public void DrawCard()
    {
        if (_deck.Count == 0)
            Shuffle();

        CardController card = Instantiate(CardPrefab, HandLayout);
        card.Set(_deck[_deck.Count - 1]);
        card.SetState(CardState.Hand);
        _handCards.Add(_deck[_deck.Count - 1]);
        _hand.Add(card);
        _deck.RemoveAt(_deck.Count - 1);
        _deckCardCount.text = _deck.Count.ToString();
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
        basement.Card.Shield.IsPlaced = false;
        _discardPile.Add(basement.Card);
        UpdateDiscardImage();
        Bases.Remove(basement);
        Destroy(basement.gameObject);
    }

    public void OnBuy(Card card)
    {
        _discardPile.Add(card);
        UpdateDiscardImage();
    }

    private void UpdateDiscardImage()
    {
        if (_discardPile.Count == 0)
        {
            _discardPileTransform.gameObject.SetActive(false);
            return;
        }
        _discardPileTransform.gameObject.SetActive(true);
        foreach (var child in _discardPileTransform.GetComponentsInChildren<CardController>())
        {
            Destroy(child.gameObject);
        }
        var discardlastCard = Instantiate(CardPrefab, _discardPileTransform);
        discardlastCard.Set(_discardPile[_discardPile.Count - 1]);
        discardlastCard.SetState(CardState.DiscardPile);
    }

    private void Shuffle()
    {
        var allCards = new List<Card>();
        allCards.AddRange(_deck);
        allCards.AddRange(_discardPile);
        _discardPile.Clear();
        UpdateDiscardImage();
        _deck.Clear();
        _deck.AddRange(allCards);
        CardSystem.instance.Shuffle(ref _deck);
    }

    public void PlaceBase(CardController card)
    {
        Bases.Add(card);
        card.Parent = BaseLayout;
    }
}
