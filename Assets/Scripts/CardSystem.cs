using System.Collections.Generic;
using UnityEngine;

public class CardSystem : MonoBehaviour
{
    public static CardSystem instance;

    private void Awake()
    {
        instance = this;
        InitStartingDeck();
        InitTradeDeck();

        void InitStartingDeck()
        {
            StartingDeck = new List<Card>();

            for (int i = 0; i < 8; i++)
                StartingDeck.Add(new Card("Scout", 0, Faction.None,
                    new List<Effect>() { new Effect(EffectGroup.Priamry, EffectType.Trade, 1) }));

            for (int i = 0; i < 2; i++)
                StartingDeck.Add(new Card("Viper", 0, Faction.None,
                    new List<Effect>() { new Effect(EffectGroup.Priamry, EffectType.Combat, 1) }));

        }

        void InitTradeDeck()
        {
            _tradeDeck = new List<Card>();

            for (int i = 0; i < 5; i++)
            {
                _tradeDeck.Add(new Card("Stinger", 0, Faction.None,
                   new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Combat, 3),
                    new Effect(EffectGroup.Ally1, EffectType.Combat, 3),
                    new Effect(EffectGroup.Scrap, EffectType.Trade, 1), }));
            }

        }
    }

    private void Start()
    {
        GameManager.instance.OnGameStart += GameStart;
    }

    public void GameStart()
    {
        InitTradeRow();
    }

    private void InitTradeRow()
    {
        if (_tradeRow == null)
            _tradeRow = new List<CardController>();
        else
            _tradeRow.Clear();

        for (int i = 0; i < 5; i++)
        {
            var newCard = Instantiate(CardPrefab, TradeRowLayout);
            _tradeRow.Add(newCard);
            newCard.SetState(CardState.TradeRow);
            Draw(i);
        }
    }

    public delegate void Scrap(Card card);
    public event Scrap OnScrap;

    public void ToScrap(Card card)
    {
        if (_tradeRow.ConvertAll(p => p.Card).Contains(card))
        {
            var index = _tradeRow.FindIndex(p => p.Card == card);
            _tradeDeck.Remove(card);
            Draw(index);
        }
        else if (_tradeDeck.Contains(card))
            _tradeDeck.Remove(card);
        OnScrap?.Invoke(card);
    }

    private void Draw(int index)
    {
        _tradeRow[index].Set(_tradeDeck[_tradeDeck.Count - 1]);
        _tradeDeck.RemoveAt(_tradeDeck.Count - 1);
    }

    public void OnBuy(CardController card)
    {
        int index = _tradeRow.FindIndex(p => p == card);

        Draw(index);
    }

    public CardController CardPrefab;

    public List<Card> StartingDeck { get; private set; }
    private List<Card> _tradeDeck;
    public Transform TradeRowLayout;
    private List<CardController> _tradeRow;
    public List<Card> TradeRow => _tradeRow.ConvertAll(p => p.Card);
}
