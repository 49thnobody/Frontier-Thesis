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
    public List<Card> HandCards => _handCards;
    public List<CardController> Hand => _hand;

    [SerializeField] private Transform BaseLayout;
    public List<CardController> Bases;

    [SerializeField] private TextMeshProUGUI _deckCardCount;
    private List<Card> _deck;
    private List<Card> _discardPile;
    public List<Card> DiscardPile => _discardPile;

    [SerializeField] private Transform _discardPileTransform;
    private CardController _discardLastCard;

    [SerializeField] private Button _playAllCards;

    private void Awake()
    {
        instance = this;
        _discardPile = new List<Card>();
        _hand = new List<CardController>();
        _handCards = new List<Card>();
        _deck = new List<Card>();
        Bases = new List<CardController>();

        _playAllCards.onClick.AddListener(PlayAllCards);
    }

    private void Start()
    {
        GameManager.instance.OnGameStart += GameStart;

        CardSystem.instance.OnScrap += OnScrap;
    }

    private void PlayAllCards()
    {
        for (int i = 0; i < _hand.Count; i++)
        {
            if (_hand[i].Card.Effects.FindAll(p => p.Type.ToString().Contains("Scrap")).Count > 0)
                continue;
            PlayAreaController.instance.PlayCard(_hand[i]);
        }
    }

    public void TakeDamage(int damage)
    {
        Authority.UpdateValue(Authority.Value - damage);
        if (Authority.Value <= 0)
            EndGameController.instance.Show(true);
    }

    public void AddAuthority(int authority)
    {
        if (authority < 0) return;
        Authority.UpdateValue(Authority.Value + authority);
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
        if (Bases.ConvertAll(p => p.Card).Contains(card))
        {
            var cardC = Bases.Find(p => p.Card == card);
            Bases.Remove(cardC);
            Destroy(cardC.gameObject);
        }
    }

    public void GameStart()
    {
        _discardLastCard = Instantiate(CardPrefab, _discardPileTransform);
        UpdateDiscardImage();
        Reset();
        foreach (var card in CardSystem.instance.StartingDeck)
        {
            _deck.Add(card.Clone() as Card);
        }
        Shuffle();

        for (int i = 0; i < 5; i++)
        {
            DrawCard();
        }
    }

    public void EndTurn()
    {
        _discardCount = 0;
        _discardPile.AddRange(_handCards.FindAll(p => p.Shield == null));
        _handCards.Clear();

        for (int i = _hand.Count - 1; i >= 0; i--)
        {
            if (!_hand[i].IsBase || !_hand[i].Card.Shield.IsPlaced)
                Destroy(_hand[i].gameObject);

            _hand.RemoveAt(i);
        }
        UpdateDiscardImage();

        for (int i = 0; i < 5; i++)
        {
            DrawCard();
        }

        _playAllCards.enabled = false;
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

        Card card = _deck[_deck.Count - 1];
        foreach (var effect in card.Effects)
        {
            effect.IsApplied = false;
        }
        if (card.Shield != null) card.Shield.IsPlaced = false;
        CardController cardC = Instantiate(CardPrefab, HandLayout);
        cardC.Set(card);
        cardC.SetState(CardState.Hand);
        _handCards.Add(card);
        _hand.Add(cardC);
        _deck.RemoveAt(_deck.Count - 1);
        _deckCardCount.text = _deck.Count.ToString();
    }

    private int _discardCount;
    public void DiscardOnStart()
    {
        _discardCount++;
    }

    public void StartTurn()
    {
        _playAllCards.enabled = true;
        if (_discardCount == 0) return;
        if (EnemyController.instance.Authority.Value <= 0) return;
        if (Authority.Value <= 0) return;
        DiscardController.instance.Show(_handCards, _discardCount);
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
        _discardLastCard.Set(_discardPile[_discardPile.Count - 1]);
        _discardLastCard.SetState(CardState.DiscardPile);
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
        card.Place(BaseLayout);
        card.SetState(CardState.Basement);
    }
}
