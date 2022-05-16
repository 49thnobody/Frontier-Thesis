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
            newCard.Set(_tradeDeck[_tradeDeck.Count - 1 - i]);
            _tradeRow.Add(newCard);
        }
    }


    public CardController CardPrefab;

    public List<Card> StartingDeck { get; private set; }
    private List<Card> _tradeDeck;
    public Transform TradeRowLayout;
    private List<CardController> _tradeRow;
}
