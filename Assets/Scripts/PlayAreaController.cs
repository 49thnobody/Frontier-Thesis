using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayAreaController : MonoBehaviour, IDropHandler
{
    public static PlayAreaController instance;

    private void Awake()
    {
        instance = this;
        _cards = new List<Card>();
    }

    [SerializeField] private OutputController Trade;
    private int _trade;
    [SerializeField] private OutputController Combat;
    private int _combat;
    [SerializeField] private OutputController Authority;
    private int _authority;

    [SerializeField] private Transform CardLayout;

    [SerializeField] private Button ButtonEndTurn;

    private List<Card> _cards;
    private Dictionary<Faction, int> _factions;

    private Turn _turn;

    private void Start()
    {
        ButtonEndTurn.onClick.AddListener(EndTurn);
        GameManager.instance.OnGameStart += GameStart;
        _factions = new Dictionary<Faction, int>();
        _factions.Add(Faction.Slugs, 0);
        _factions.Add(Faction.Technocult, 0);
        _factions.Add(Faction.StarEmpire, 0);
        _factions.Add(Faction.TradeFederation, 0);
    }

    public void GameStart()
    {
        _turn = Turn.PlayerTurn;
        _cards.Clear();
        _trade = _combat = _authority = 0;
        Trade.UpdateValue(_trade);
        Combat.UpdateValue(_combat);
        Authority.UpdateValue(_authority);
    }

    public void EndTurn()
    {
        switch (_turn)
        {
            case Turn.PlayerTurn:
                _cards.AddRange(PlayerController.instance.Bases.ConvertAll(p => p.Card));

                var enemyOutposts = EnemyController.instance.Bases.FindAll(p => p.Card.Shield.Type == ShieldType.Outpost);
                foreach (var outpost in enemyOutposts)
                {
                    if (outpost.Card.Shield.HP > _combat)
                    {
                        _combat = 0;
                        break;
                    }
                    else
                    {
                        _combat -= outpost.Card.Shield.HP;
                        EnemyController.instance.DiscardBase(outpost);
                    }
                }

                EnemyController.instance.TakeDamage(_combat);
                PlayerController.instance.EndTurn();
                EnemyController.instance.OnMyTurn();
                break;
            case Turn.EnemyTurn:
                _cards.AddRange(EnemyController.instance.Bases.ConvertAll(p => p.Card));

                var playerOutposts = PlayerController.instance.Bases.FindAll(p => p.Card.Shield.Type == ShieldType.Outpost);
                foreach (var outpost in playerOutposts)
                {
                    if (outpost.Card.Shield.HP > _combat)
                    {
                        _combat = 0;
                        break;
                    }
                    else
                    {
                        _combat -= outpost.Card.Shield.HP;
                        PlayerController.instance.DiscardBase(outpost);
                    }
                }

                PlayerController.instance.TakeDamage(_combat);
                break;
            default:
                break;
        }

        Reset();
    }

    private void Reset()
    {
        _trade = _combat = _authority = 0;
        Trade.UpdateValue(_trade);
        Combat.UpdateValue(_combat);
        Authority.UpdateValue(_authority);
        _cards.Clear();

        var cardCs = CardLayout.GetComponentsInChildren<CardController>();
        for (int i = cardCs.Length - 1; i >= 0; i--)
        {
            Destroy(cardCs[i].gameObject);
        }
    }

    public void PlayCard(Card card)
    {
        _cards.Add(card);

        foreach (var effect in card.PrimaryEffects)
        {
            PlayEffect(effect);
        }

        if (card.Faction != Faction.None)
        {
            _factions[card.Faction]++;

            PlayFactionEffects();
        }
    }

    private void PlayFactionEffects()
    {
        var cardsWithFactionEffect = _cards.FindAll(p => p.Ally1Effects.Count != 0);
        foreach (var card in cardsWithFactionEffect)
        {
            if (_factions[card.Faction] > 1)
            {
                foreach (var effect in card.Ally1Effects)
                {
                    if (!effect.IsApplied) PlayEffect(effect);
                }
            }

            if (_factions[card.Faction] > 2)
            {
                foreach (var effect in card.Allly2Effects)
                {
                    if (!effect.IsApplied) PlayEffect(effect);
                }
            }
        }
    }

    public void PlayEffect(Effect effect)
    {
        switch (effect.Type)
        {
            case EffectType.Trade:
                _trade += effect.Value;
                Trade.UpdateValue(_trade);

                effect.IsApplied = true;
                break;
            case EffectType.Combat:
                _combat += effect.Value;
                Combat.UpdateValue(_combat);

                effect.IsApplied = true;
                break;
            case EffectType.Authority:
                _authority += effect.Value;
                Authority.UpdateValue(_authority);

                effect.IsApplied = true;
                break;
            case EffectType.Draw:
                switch (_turn)
                {
                    case Turn.PlayerTurn:
                        PlayerController.instance.DrawCard();
                        effect.IsApplied = true;
                        break;
                    case Turn.EnemyTurn:
                        EnemyController.instance.DrawCard();
                        effect.IsApplied = true;
                        break;
                    default:
                        break;
                }
                break;
            case EffectType.Discard:
                // only enemy have have this
                EnemyController.instance.DiscardCard();
                break;
            case EffectType.ForceToDiscard:
                // only enemy have this type of effect
                PlayerController.instance.DiscardOnStart();
                effect.IsApplied = true;
                break;
            case EffectType.ScrapFromHand:
                List<Card> cardsToSelectHand = new List<Card>();
                cardsToSelectHand.AddRange(PlayerController.instance.Hand);

                ScrapController.instance.Show(cardsToSelectHand, effect.Value, effect);
                break;
            case EffectType.ScrapFromDiscardPile:
                List<Card> cardsToSelectDiscard = new List<Card>();
                cardsToSelectDiscard.AddRange(PlayerController.instance.DiscardPile);

                ScrapController.instance.Show(cardsToSelectDiscard, effect.Value, effect);
                break;
            case EffectType.ScrapFromHandOrDiscardPile:
                List<Card> cardsToSelectAny = new List<Card>();
                cardsToSelectAny.AddRange(PlayerController.instance.DiscardPile);
                cardsToSelectAny.AddRange(PlayerController.instance.Hand);

                ScrapController.instance.Show(cardsToSelectAny, effect.Value, effect);
                break;
            case EffectType.ScrapFromTradeRow:
                List<Card> cardsToSelectTrade = new List<Card>();
                cardsToSelectTrade.AddRange(CardSystem.instance.TradeRow);

                ScrapController.instance.Show(cardsToSelectTrade, effect.Value, effect);
                break;
            default:
                break;
        }
    }

    public bool BuyCard(Card card)
    {
        if (_trade < card.Cost) return false;

        _trade -= card.Cost;
        Trade.UpdateValue(_trade);
        return true;
    }

    public void PlaceEnemyCard(Card card)
    {

    }

    public void OnDrop(PointerEventData eventData)
    {
        CardController card = eventData.pointerDrag.GetComponent<CardController>();

        if (!card) return;

        if (card.IsBase)
        {
            switch (_turn)
            {
                case Turn.PlayerTurn:
                    PlayerController.instance.PlaceBase(card);
                    break;
                case Turn.EnemyTurn:
                    EnemyController.instance.PlaceBase(card);
                    break;
                default:
                    break;
            }
        }
        else
            card.Parent = CardLayout;

        PlayCard(card.Card);
    }
}