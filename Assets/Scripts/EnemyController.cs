using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // ��������
    public static EnemyController instance;

    // �����, ��� ������������� ����
    public Transform BaseLayout;
    // ������ ���
    public List<CardController> Bases;

    // ���� ����������
    public OutputController Authority;

    // ������ �����
    public CardController CardPrefab;
    // ����� � ����
    private List<Card> _handCards;

    // ������ ������
    private List<Card> _deck;
    // ������ ������
    private List<Card> _discardPile;

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
        // �������� �� ������� - ������ ����
        GameManager.instance.OnGameStart += GameStart;
    }

    // �����
    private void EndTurn()
    {
        // ����� ����
        _discardPile.AddRange(_handCards);
        // ������� ����
        _handCards.Clear();

        // ����� ����� ���� 
        for (int i = 0; i < 5; i++)
        {
            DrawCard();
        }

        // ����� ����� ���� - ��� �������� ���� ������
        PlayAreaController.instance.OnEndTurn();
    }

    // ����� �����
    public void DrawCard()
    {
        // ���� � ������ �� �������� ���� - ������������ �
        if (_deck.Count == 0)
            Shuffle();

        // ����� �����
        int last = _deck.Count - 1;
        _handCards.Add(_deck[last]);
        // ��������� � ������� - ��� � ���� ���� ��� ����� �� �����������
        foreach (var effect in _deck[last].Effects)
        {
            effect.IsApplied = false;
        }
        // ������� ����� �� ������
        _deck.RemoveAt(last);
        // ��������� ���� - ���� ����� ����������� ����� � �����, � ����������� �� ��
        // � ������� �������� ��������
        _handCards.Sort(delegate (Card card1, Card card2)
        {
            return card1.Cost.CompareTo(card2.Cost);
        });
    }

    // ����� ����� - ������� �����
    private void Discard(Card card)
    {
        if (_handCards.Contains(card))
        {
            _handCards.Remove(card);
            _discardPile.Add(card);
        }
    }

    // ���������� ����
    public void PlaceBase(CardController card)
    {
        Bases.Add(card);
        card.Place(BaseLayout);
        card.SetState(CardState.Basement);
    }

    // ����� ���� - � �������
    public void DiscardBase(CardController basement)
    {
        // ��������� �� � �����
        _discardPile.Add(basement.Card);
        // ������� �� ������
        Bases.Remove(basement);
        // ���������� ������
        Destroy(basement.gameObject);
    }

    // ��������� �����
    public void TakeDamage(int damage)
    {
        // ���������� ��������
        if (damage < 0) return;
        // ������� ���������
        Authority.UpdateValue(Authority.Value - damage);
        // ��������� ���� �� ���� ��� ���� - ��������� ��������
        if (Authority.Value <= 0)
            EndGameController.instance.Show(true);
    }

    // �������������
    private void Shuffle()
    {
        // �������� ����� ������
        var allCards = new List<Card>();
        allCards.AddRange(_deck);
        allCards.AddRange(_discardPile);
        _discardPile.Clear();
        _deck.AddRange(allCards);
        // ������ � � ����� ��� �������������
        CardSystem.instance.Shuffle(ref _deck);
    }

    // ������ ����
    public void StartTurn()
    {
        // ������ ������ ��������
        StartCoroutine(OnMyTurn());
    }

    // ��� ���
    public IEnumerator OnMyTurn()
    {
        // ���� � ���� ���� �����
        while (_handCards.Count > 0)
        {
            // ����������� ��������� �����
            PlayAreaController.instance.PlaceEnemyCard(_handCards[_handCards.Count - 1]);
            Discard(_handCards[_handCards.Count - 1]);
            // ���� ���� ���
            yield return new WaitForSeconds(1);
        }

        // �������� �����
        PlayAreaController.instance.EnemyBuy();
        // ����� ����
        EndTurn();
    }

    // ������� �����
    public void OnBuy(Card card)
    {
        _discardPile.Add(card);
    }

    // ������ ���� 
    public void GameStart()
    {
        // ���������� �� � ���� 
        Reset();
        // �������� ���� ��������� ������
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

    // ����� �� ��������� ��������
    private void Reset()
    {
        // ���� ����������
        Authority.UpdateValue(40);

        // ������� ����
        _handCards.Clear();
        // ������ ������
        _discardPile.Clear(); 
        // � ������
        _deck.Clear();

        // ���������� ���� ����
        for (int i = Bases.Count - 1; i >= 0; i--)
        {
            Destroy(Bases[i]);
            Bases.RemoveAt(i);
        }
    }

    // ����� �����
    public void DiscardCard()
    {
        if (_handCards.Count == 0) return;

        // ���� ����� ������� �����
        // ������ ��� ������������� �� �����������
        // ������� �� ������ ���� ��� ����� � ���������� ����������
        int i = 0;
        var cardsToDiscard = new List<Card>();
        do
        {
            cardsToDiscard.Add(_handCards[i]);
            i++;
        } while (i < _handCards.Count && _handCards[i].Cost == _handCards[i - 1].Cost);

        // ����� ��������� ����� �� ����� ����� �������
        int index = UnityEngine.Random.Range(0, cardsToDiscard.Count);

        // ���������� �
        Discard(cardsToDiscard[index]);
    }
}
