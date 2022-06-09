using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSystem : MonoBehaviour
{
    public static CardSystem instance;

    public CardController CardPrefab;

    public List<Card> StartingDeck;
    private List<Card> _tradeDeck;
    public Transform TradeRowLayout;
    private List<CardController> _tradeRow;
    public List<Card> TradeRow => _tradeRow.ConvertAll(p => p.Card);
    public List<Card> EmperorsElitesTradeDeck;
    public List<Card> EmperorsSubjectsTradeDeck;

    [SerializeField] private Transform _elitesDeckNextBuy;
    private CardController _eliteLastCard;
    [SerializeField] private Transform _subjectsDeckNextBuy;
    private CardController _subjectLastCard;
    private void Awake()
    {
        instance = this;
        InitStartingDeck();
        InitTradeDeck();
        Shuffle(ref _tradeDeck);
        Shuffle(ref EmperorsSubjectsTradeDeck);

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

            #region Slugs
            _tradeDeck.Add(new Card("BlobAlpha", 6, Faction.Slugs,
                   new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Combat, 10)
                   }));
            for (int i = 0; i < 3; i++)
            {
                _tradeDeck.Add(new Card("BlobMiner", 2, Faction.Slugs,
                    new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Trade, 3),
                    new Effect(EffectGroup.Priamry, EffectType.ScrapFromTradeRow, 1),
                    new Effect(EffectGroup.Scrap, EffectType.Combat, 2)
                   }));
            }
            for (int i = 0; i < 2; i++)
            {
                _tradeDeck.Add(new Card("Burrower", 3, Faction.Slugs,
                    new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Combat, 5),
                    new Effect(EffectGroup.Ally1, EffectType.Draw, 1),
                    new Effect(EffectGroup.Scrap, EffectType.Trade, 4),
                   }));
            }
            for (int i = 0; i < 2; i++)
            {
                _tradeDeck.Add(new Card("Crusher", 3, Faction.Slugs,
                   new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Combat, 6),
                    new Effect(EffectGroup.Ally1, EffectType.Trade, 2)
                   }));
            }
            _tradeDeck.Add(new Card("HiveQueen", 7, Faction.Slugs,
               new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Combat, 7),
                    new Effect(EffectGroup.Priamry, EffectType.Draw, 1),
                    new Effect(EffectGroup.Ally1, EffectType.Combat, 3),
                    new Effect(EffectGroup.Ally2, EffectType.Combat, 3),
               }));
            _tradeDeck.Add(new Card("InfestedMoon", 6, Faction.Slugs,
               new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Combat, 4),
                    new Effect(EffectGroup.Ally1, EffectType.Draw, 3),
                    new Effect(EffectGroup.Ally2, EffectType.Draw, 1),
               }, new Shield(ShieldType.White, 5)));
            for (int i = 0; i < 2; i++)
            {
                _tradeDeck.Add(new Card("MoonwurmHatching", 4, Faction.Slugs,
                   new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Trade, 3),
                    new Effect(EffectGroup.Ally1, EffectType.Combat, 3),
                    new Effect(EffectGroup.Ally2, EffectType.Combat, 3),
                   }));
            }
            _tradeDeck.Add(new Card("NestingGround", 4, Faction.Slugs,
               new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Trade, 2),
                    new Effect(EffectGroup.Ally1, EffectType.Combat, 4),
               }, new Shield(ShieldType.White, 5)));
            _tradeDeck.Add(new Card("Pulverizer", 5, Faction.Slugs,
               new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Combat, 5),
                    new Effect(EffectGroup.Priamry, EffectType.ScrapFromTradeRow, 1),
                    new Effect(EffectGroup.Ally1, EffectType.Draw, 1),
               }));
            for (int i = 0; i < 2; i++)
            {
                _tradeDeck.Add(new Card("SpikeCluster", 2, Faction.Slugs,
                    new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Combat, 2),
                    new Effect(EffectGroup.Ally1, EffectType.Trade, 1),
                   }, new Shield(ShieldType.White, 3)));
            }
            for (int i = 0; i < 3; i++)
            {
                _tradeDeck.Add(new Card("Stinger", 1, Faction.Slugs,
                   new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Combat, 3),
                    new Effect(EffectGroup.Ally1, EffectType.Combat, 3),
                    new Effect(EffectGroup.Scrap, EffectType.Trade, 1),
                   }));
            }
            _tradeDeck.Add(new Card("SwarmCluster", 8, Faction.Slugs,
               new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Combat, 5),
                    new Effect(EffectGroup.Ally1, EffectType.Combat, 3),
                    new Effect(EffectGroup.Ally2, EffectType.Combat, 3),
               }, new Shield(ShieldType.White, 8)));
            #endregion

            #region Technocult
            for (int i = 0; i < 3; i++)
            {
                _tradeDeck.Add(new Card("BuilderBot", 1, Faction.Technocult,
                   new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Trade, 1),
                    new Effect(EffectGroup.Priamry, EffectType.ScrapFromDiscardPile, 1),
                    new Effect(EffectGroup.Ally1, EffectType.Trade, 1),
                    new Effect(EffectGroup.Scrap, EffectType.Combat, 2),
                   }));
            }
            _tradeDeck.Add(new Card("ConversationYard", 5, Faction.Technocult,
               new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Combat, 4)
               }, new Shield(ShieldType.Outpost, 4)));
            for (int i = 0; i < 2; i++)
            {
                _tradeDeck.Add(new Card("DefenceSystem", 4, Faction.Technocult,
                   new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Combat, 2),
                    new Effect(EffectGroup.Ally1, EffectType.Combat, 2),
                   }, new Shield(ShieldType.Outpost, 5)));
            }
            for (int i = 0; i < 3; i++)
            {
                _tradeDeck.Add(new Card("DestroyerBot", 3, Faction.Technocult,
                   new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Combat, 5),
                    new Effect(EffectGroup.Priamry, EffectType.ScrapFromDiscardPile, 1),
                   }));
            }
            _tradeDeck.Add(new Card("EnforcerMech", 5, Faction.Technocult,
               new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Combat, 5),
                    new Effect(EffectGroup.Priamry, EffectType.ScrapFromHandOrDiscardPile, 1),
                    new Effect(EffectGroup.Ally1, EffectType.Combat, 4),
                    new Effect(EffectGroup.Scrap, EffectType.Draw, 1),
               }));
            for (int i = 0; i < 2; i++)
            {
                _tradeDeck.Add(new Card("IntegrationPort", 3, Faction.Technocult,
                   new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Trade, 1),
                   }, new Shield(ShieldType.Outpost, 5)));
            }
            _tradeDeck.Add(new Card("NanobotSwarm", 8, Faction.Technocult,
               new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Combat, 5),
                    new Effect(EffectGroup.Priamry, EffectType.ScrapFromHandOrDiscardPile, 2),
                    new Effect(EffectGroup.Priamry, EffectType.Draw, 2),
               }));
            _tradeDeck.Add(new Card("NeuralNexus", 7, Faction.Technocult,
               new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Combat, 2),
                    new Effect(EffectGroup.Priamry, EffectType.ScrapFromHandOrDiscardPile, 1),
                    new Effect(EffectGroup.Ally1, EffectType.Draw, 1),
               }, new Shield(ShieldType.Outpost, 6)));
            for (int i = 0; i < 3; i++)
            {
                _tradeDeck.Add(new Card("PlasmaBot", 2, Faction.Technocult,
                  new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Combat, 3),
                    new Effect(EffectGroup.Priamry, EffectType.ScrapFromHand, 1),
                    new Effect(EffectGroup.Ally1, EffectType.Combat, 2),
                  }));
            }
            _tradeDeck.Add(new Card("ReclamationStation", 6, Faction.Technocult,
               new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.ScrapFromDiscardPile, 1),
                    new Effect(EffectGroup.Scrap, EffectType.Combat, 7),
               }, new Shield(ShieldType.Outpost, 6)));
            #endregion

            #region TradeFederation
            _tradeDeck.Add(new Card("FederationBattleship", 7, Faction.TradeFederation,
               new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Combat, 5),
                    new Effect(EffectGroup.Priamry, EffectType.Authority, 5),
                    new Effect(EffectGroup.Priamry, EffectType.Draw, 1),
                    new Effect(EffectGroup.Ally1, EffectType.Combat, 6),
                    new Effect(EffectGroup.Scrap, EffectType.Authority, 10),
               }));
            _tradeDeck.Add(new Card("FederationCruiser", 5, Faction.TradeFederation,
               new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Combat, 5),
                    new Effect(EffectGroup.Priamry, EffectType.Authority, 4),
                    new Effect(EffectGroup.Ally1, EffectType.Combat, 2),
                    new Effect(EffectGroup.Ally1, EffectType.Authority, 2),
               }));
            for (int i = 0; i < 3; i++)
            {
                _tradeDeck.Add(new Card("FrontierRunner", 1, Faction.TradeFederation,
                  new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Trade, 2),
                    new Effect(EffectGroup.Priamry, EffectType.Authority, 2),
                  }));
            }
            _tradeDeck.Add(new Card("Gateship", 6, Faction.TradeFederation,
              new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Trade, 6),
                    new Effect(EffectGroup.Ally1, EffectType.Authority, 5),
              }));
            _tradeDeck.Add(new Card("IonStation", 5, Faction.TradeFederation,
              new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Trade, 2),
                    new Effect(EffectGroup.Ally1, EffectType.Trade, 1),
                    new Effect(EffectGroup.Ally2, EffectType.Combat, 4),
                    new Effect(EffectGroup.Ally2, EffectType.Authority, 4),
              }, new Shield(ShieldType.Outpost, 5)));
            for (int i = 0; i < 2; i++)
            {
                _tradeDeck.Add(new Card("LongHauler", 4, Faction.TradeFederation,
                   new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Trade, 3),
                    new Effect(EffectGroup.Ally1, EffectType.Trade, 2),
                    new Effect(EffectGroup.Scrap, EffectType.Draw, 1),
                   }));
            }
            for (int i = 0; i < 2; i++)
            {
                _tradeDeck.Add(new Card("MobileMarket", 4, Faction.TradeFederation,
                   new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Trade, 2),
                    new Effect(EffectGroup.Scrap, EffectType.Authority, 2),
                    new Effect(EffectGroup.Scrap, EffectType.Draw, 1),
                   }, new Shield(ShieldType.Outpost, 4)));
            }
            for (int i = 0; i < 3; i++)
            {
                _tradeDeck.Add(new Card("OrbitalShuttle", 2, Faction.TradeFederation,
                  new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Trade, 3),
                    new Effect(EffectGroup.Priamry, EffectType.Draw, 1),
                  }));
            }
            for (int i = 0; i < 3; i++)
            {
                _tradeDeck.Add(new Card("OutlandStation", 3, Faction.TradeFederation,
                   new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Trade, 1),
                    new Effect(EffectGroup.Priamry, EffectType.Authority, 2),
                    new Effect(EffectGroup.Scrap, EffectType.Draw, 1),
                   }, new Shield(ShieldType.White, 4)));
            }
            for (int i = 0; i < 2; i++)
            {
                _tradeDeck.Add(new Card("PatrolBoat", 3, Faction.TradeFederation,
                   new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Combat, 4),
                    new Effect(EffectGroup.Priamry, EffectType.Authority, 3),
                    new Effect(EffectGroup.Ally1, EffectType.Authority, 2),
                   }));
            }
            _tradeDeck.Add(new Card("TransitNexus", 8, Faction.TradeFederation,
               new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Combat, 3),
                    new Effect(EffectGroup.Priamry, EffectType.Trade, 4),
                    new Effect(EffectGroup.Priamry, EffectType.Authority, 5),
               }, new Shield(ShieldType.White, 6)));
            #endregion

            #region StarEmpire
            EmperorsElitesTradeDeck = new List<Card>();
            EmperorsElitesTradeDeck.Add(new Card("ImperialFlagship", 8, Faction.StarEmpire,
               new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Combat, 7),
                    new Effect(EffectGroup.Priamry, EffectType.Draw, 2),
                    new Effect(EffectGroup.Ally1, EffectType.ForceToDiscard, 1),
               }));
            EmperorsElitesTradeDeck.Add(new Card("SiegeFortress", 7, Faction.StarEmpire,
               new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Combat, 5),
                    new Effect(EffectGroup.Ally1, EffectType.Combat, 4),
               }, new Shield(ShieldType.Outpost, 5)));
            EmperorsElitesTradeDeck.Add(new Card("JammingTerminal", 5, Faction.StarEmpire,
               new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Combat, 2),
                    new Effect(EffectGroup.Priamry, EffectType.ForceToDiscard, 1),
               }, new Shield(ShieldType.White, 6)));
            EmperorsElitesTradeDeck.Add(new Card("Hammerhead", 5, Faction.StarEmpire,
               new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Combat, 3),
                    new Effect(EffectGroup.Ally1, EffectType.Draw, 1),
                    new Effect(EffectGroup.Ally1, EffectType.ForceToDiscard, 1),
               }));

            EmperorsSubjectsTradeDeck = new List<Card>();
            for (int i = 0; i < 2; i++)
            {
                EmperorsSubjectsTradeDeck.Add(new Card("CapturedOutpost", 3, Faction.StarEmpire,
                   new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Draw, 1),
                    new Effect(EffectGroup.Priamry, EffectType.Discard, 1),
                   }, new Shield(ShieldType.Outpost, 3)));
            }
            for (int i = 0; i < 3; i++)
            {
                EmperorsSubjectsTradeDeck.Add(new Card("CargoCraft", 2, Faction.StarEmpire,
                   new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Trade, 2),
                    new Effect(EffectGroup.Priamry, EffectType.ForceToDiscard, 1),
                    new Effect(EffectGroup.Ally1, EffectType.Draw, 1),
                   }));
            }
            for (int i = 0; i < 2; i++)
            {
                EmperorsSubjectsTradeDeck.Add(new Card("FarmShip", 4, Faction.StarEmpire,
                   new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Trade, 3),
                    new Effect(EffectGroup.Priamry, EffectType.Draw, 1),
                    new Effect(EffectGroup.Priamry, EffectType.Discard, 1),
                    new Effect(EffectGroup.Scrap, EffectType.Combat, 4),
                   }));
            }
            for (int i = 0; i < 3; i++)
            {
                EmperorsSubjectsTradeDeck.Add(new Card("FrontierHawk", 1, Faction.StarEmpire,
                   new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Combat, 3),
                    new Effect(EffectGroup.Priamry, EffectType.Draw, 1),
                    new Effect(EffectGroup.Priamry, EffectType.Discard, 1),
                   }));
            }
            for (int i = 0; i < 3; i++)
            {
                EmperorsSubjectsTradeDeck.Add(new Card("LightCruiser", 3, Faction.StarEmpire,
                   new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Combat, 4),
                    new Effect(EffectGroup.Priamry, EffectType.ForceToDiscard, 1),
                    new Effect(EffectGroup.Ally1, EffectType.Combat, 2),
                    new Effect(EffectGroup.Ally2 , EffectType.Draw, 1),
                   }));
            }
            for (int i = 0; i < 2; i++)
            {
                EmperorsSubjectsTradeDeck.Add(new Card("OrbitalGunPlatform", 4, Faction.StarEmpire,
                       new List<Effect>() {
                    new Effect(EffectGroup.Priamry, EffectType.Combat, 3),
                    new Effect(EffectGroup.Scrap, EffectType.Trade, 3),
                       }, new Shield(ShieldType.Outpost, 4)));
            }
            #endregion
        }
    }

    public void Shuffle(ref List<Card> cardsToShuffle)
    {
        int cardCount = cardsToShuffle.Count;
        var newList = new List<Card>();

        while (newList.Count < cardCount)
        {
            int currentIndex = Random.Range(0, cardsToShuffle.Count);

            newList.Add(cardsToShuffle[currentIndex]);
            cardsToShuffle.RemoveAt(currentIndex);
        }

        cardsToShuffle = newList;
    }


    private void Start()
    {
        GameManager.instance.OnGameStart += GameStart;
    }

    public void GameStart()
    {
        InitTradeRow();

        _eliteLastCard = Instantiate(CardPrefab, _elitesDeckNextBuy);
        _eliteLastCard.Set(EmperorsElitesTradeDeck[EmperorsElitesTradeDeck.Count - 1]);
        _eliteLastCard.SetState(CardState.EnemyBuy);
        _subjectLastCard = Instantiate(CardPrefab, _subjectsDeckNextBuy);
        _subjectLastCard.Set(EmperorsSubjectsTradeDeck[EmperorsSubjectsTradeDeck.Count - 1]);
        _subjectLastCard.SetState(CardState.EnemyBuy);

    }

    private void InitTradeRow()
    {
        if (_tradeRow == null)
            _tradeRow = new List<CardController>();
        else
        {
            for (int i = _tradeRow.Count - 1; i >= 0; i--)
            {
                Destroy(_tradeRow[i].gameObject);
                _tradeRow.RemoveAt(i);
            }
        }

        for (int i = 0; i < 5; i++)
        {
            var newCard = Instantiate(CardPrefab, TradeRowLayout);
            _tradeRow.Add(newCard);
            Draw(i);
        }

        _eliteLastCard = Instantiate(CardPrefab, _elitesDeckNextBuy);
        _eliteLastCard.Set(EmperorsElitesTradeDeck[EmperorsElitesTradeDeck.Count - 1]);
        _eliteLastCard.SetState(CardState.EnemyBuy);

        _subjectLastCard = Instantiate(CardPrefab, _subjectsDeckNextBuy);
        _subjectLastCard.Set(EmperorsSubjectsTradeDeck[EmperorsSubjectsTradeDeck.Count - 1]);
        _subjectLastCard.SetState(CardState.EnemyBuy);
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
        _tradeRow[index].SetState(CardState.TradeRow);
        _tradeDeck.RemoveAt(_tradeDeck.Count - 1);
    }

    public void OnPlayerBuy(CardController card)
    {
        int index = _tradeRow.FindIndex(p => p == card);

        Draw(index);
    }

    public void BuyMostExpansive(ref int trade)
    {
        var affordable = new List<Card>();

        Card elite = null;
        if (EmperorsElitesTradeDeck.Count > 0)
            elite = EmperorsElitesTradeDeck[EmperorsElitesTradeDeck.Count - 1];
        if (elite != null && elite.Cost <= trade)
            affordable.Add(elite);
        Card subject = null;
        if (EmperorsSubjectsTradeDeck.Count > 0)
            subject = EmperorsSubjectsTradeDeck[EmperorsSubjectsTradeDeck.Count - 1];
        if (subject != null && subject.Cost <= trade)
            affordable.Add(subject);

        if (affordable.Count == 0)
        {
            trade = 0;
            return;
        }

        affordable.Sort(delegate (Card card1, Card card2)
        {
            return card2.Cost.CompareTo(card1.Cost);
        });
        Card mostExpansive = affordable[0];

        if (mostExpansive == elite)
        {
            EmperorsElitesTradeDeck.Remove(elite);
            if (EmperorsElitesTradeDeck.Count > 0)
            {
                _eliteLastCard.Set(EmperorsElitesTradeDeck[EmperorsElitesTradeDeck.Count - 1]);
                _eliteLastCard.SetState(CardState.EnemyBuy);
            }
        }
        if (mostExpansive == subject)
        {
            EmperorsSubjectsTradeDeck.Remove(subject);
            if (EmperorsSubjectsTradeDeck.Count > 0)
            {
                _subjectLastCard.Set(EmperorsSubjectsTradeDeck[EmperorsSubjectsTradeDeck.Count - 1]);
                _subjectLastCard.SetState(CardState.EnemyBuy);
            }
        }

        trade -= mostExpansive.Cost;
        EnemyController.instance.OnBuy(mostExpansive);
    }
}
