using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // ��������
    public static PlayerController instance;

    // ���������
    [SerializeField] private OutputController _authority;
    // �������� ��� ���� � ����
    [SerializeField] private Transform _handLayout;
    // ������ �����
    [SerializeField] private CardController _cardPrefab;

    // ���� (����� � ������������
    #region Hand
    private List<CardController> _hand;
    private List<Card> _handCards;
    public List<Card> HandCards => _handCards;
    public List<CardController> Hand => _hand;
    #endregion

    // �������� ��� ���
    [SerializeField] private Transform _baseLayout;
    // ����
    public List<CardController> Bases;

    // ���������� ���� � ������ ������
    [SerializeField] private TextMeshProUGUI _deckCardCount;
    // ������ ������
    private List<Card> _deck;
    // ������ ������
    private List<Card> _discardPile;
    public List<Card> DiscardPile => _discardPile;

    // �������� ��������� ����� � ������ ������
    [SerializeField] private Transform _discardPileTransform;
    private CardController _discardLastCard;

    // ������ - ������� ��
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

    // ������� ��
    private void PlayAllCards()
    {
        for (int i = 0; i < _hand.Count; i++)
        {
            // � ������, ��� ������ �� �������� �����, ������� ��������� �������� ������, ����� �� �����
            if (_hand[i].Card.Effects.FindAll(p => p.Type.ToString().Contains("Scrap")).Count > 0)
                continue;
            PlayAreaController.instance.PlayCard(_hand[i]);
        }
    }

    // ��������� �����
    public void TakeDamage(int damage)
    {
        // ���������� ��������
        if (damage < 0) return;
        // �������� ����
        _authority.UpdateValue(_authority.Value - damage);
        // ��������� ���� �� ���� ��� ���� - ���������
        if (_authority.Value <= 0)
            EndGameController.instance.Show(false);
    }

    // ��������� ����������
    public void AddAuthority(int authority)
    {
        // ���������� ��������
        if (authority < 0) return;
        // �������� ���������
        _authority.UpdateValue(_authority.Value + authority);
        // ��������� ���� �� ���� ��� ���� - ���������
        if (_authority.Value >= 100)
            EndGameController.instance.Show(true);
    }

    // ����������
    private void OnScrap(Card card)
    {
        // ���� ����� ����� � �������
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

    // ������ ����
    public void GameStart()
    {
        // ������� ����� � ������ ������
        _discardLastCard = Instantiate(_cardPrefab, _discardPileTransform);
        UpdateDiscardImage();
        Reset();
        // �������� ���� ������
        foreach (var card in CardSystem.instance.StartingDeck)
        {
            _deck.Add(card.Clone() as Card);
        }
        // ������������
        Shuffle();

        // ����� 5 ����
        for (int i = 0; i < 5; i++)
        {
            DrawCard();
        }
    }
    
    // ����� ����
    public void EndTurn()
    {
        // ���������� ���������� ������������ � ������ ���� ����
        _discardCount = 0;
        // ����� � ���� � �����
        _discardPile.AddRange(_handCards.FindAll(p => p.Shield == null));
        // ���� �������
        _handCards.Clear();

        // ������� ������� �� ���� (������ �� ����) (��� �� ����������, ��� ���������� ��� �������� � ����, ���� � ������������ �� ����)
        for (int i = _hand.Count - 1; i >= 0; i--)
        {
            if (!_hand[i].IsBase)
                Destroy(_hand[i].gameObject);
            else if (_hand[i].State != CardState.Basement)
                Destroy(_hand[i].gameObject);

            _hand.RemoveAt(i);
        }
        UpdateDiscardImage();

        for (int i = 0; i < 5; i++)
        {
            DrawCard();
        }

        // ������ ������ ������� ��� ��������
        _playAllCards.enabled = false;
    }

    // �����
    private void Reset()
    {
        // ����� ����������
        _authority.UpdateValue(50);

        // ����������� �������� � ����
        for (int i = _hand.Count - 1; i >= 0; i--)
        {
            Destroy(_hand[i].gameObject);
            _hand.RemoveAt(i);
        }
        // ������� ����
        _handCards.Clear();

        // ���������� ����
        for (int i = Bases.Count - 1; i >= 0; i--)
        {
            Destroy(Bases[i]);
            Bases.RemoveAt(i);
        }

        // ������� �� ���������
        _deck.Clear();
        _discardPile.Clear();
        UpdateDiscardImage();
    }

    // ����� �����
    public void DrawCard()
    {
        // ������ ����� - ������������
        if (_deck.Count == 0)
            Shuffle();

        // ����� ��������� �����
        Card card = _deck[_deck.Count - 1];
        // ���������� ���� ���������� �������� �����
        foreach (var effect in card.Effects)
        {
            effect.IsApplied = false;
        }
        // ������� ����� � ����
        CardController cardC = Instantiate(_cardPrefab, _handLayout);
        // ��������������
        cardC.Set(card);
        // �������������� ���������
        cardC.SetState(CardState.Hand);
        // ���������� ���� � ������
        _handCards.Add(card);
        _hand.Add(cardC);
        // ������� ����� �� ������
        _deck.RemoveAt(_deck.Count - 1);
        // ������ ���������� ���������� ����
        _deckCardCount.text = _deck.Count.ToString();
    }

    // ����� ���������� ����� - ��������� ����� ��������� ������ ������� ����� � ������ ��� ����
    // ������� ��� � ������ ������, ������� ���� �������
    private int _discardCount;
    public void DiscardOnStart()
    {
        _discardCount++;
    }

    // ������ ����
    public void StartTurn()
    {
        // ������ ������ ����� ���������
        _playAllCards.enabled = true;
        // ���� ��� ����� ��� ����� ���������, ������
        if (_discardCount == 0) return;
        // ���� ���-�� ��� �������, ����
        if (EnemyController.instance.Authority.Value <= 0) return;
        if (_authority.Value <= 0) return;
        // �������� ���� ������ ����
        DiscardController.instance.Show(_handCards, _discardCount);
    }

    // ����� ����
    public void Discard(List<Card> cards)
    {
        // ������ �����
        foreach (var card in cards)
        {
            // ���� � ���� � ����������
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

    // ����� ����
    public void DiscardBase(CardController @base)
    {
        // ������ ������ - ������ � � ����� � ������� ��������
        _discardPile.Add(@base.Card);
        UpdateDiscardImage();
        Bases.Remove(@base);
        Destroy(@base.gameObject);
    }

    // �������
    public void OnBuy(Card card)
    {
        // ��������� ����� � �����
        _discardPile.Add(card);
        UpdateDiscardImage();
    }

    // ��������� �������� ������ �� ��������� ����� � ������
    private void UpdateDiscardImage()
    {
        // ���� ����� ������ �������� ������
        if (_discardPile.Count == 0)
        {
            _discardPileTransform.gameObject.SetActive(false);
            return;
        }
        _discardPileTransform.gameObject.SetActive(true);
        _discardLastCard.Set(_discardPile[_discardPile.Count - 1]);
        _discardLastCard.SetState(CardState.DiscardPile);
    }

    // �������������
    private void Shuffle()
    {
        // ��������� ����� ������
        var allCards = new List<Card>();
        allCards.AddRange(_deck);
        allCards.AddRange(_discardPile);
        // ������� ��
        _discardPile.Clear();
        UpdateDiscardImage();
        _deck.Clear();
        _deck.AddRange(allCards);
        // �������������
        CardSystem.instance.Shuffle(ref _deck);
    }

    // ���������� ����
    public void PlaceBase(CardController card)
    {
        Bases.Add(card);
        card.Place(_baseLayout);
        card.SetState(CardState.Basement);
    }
}
