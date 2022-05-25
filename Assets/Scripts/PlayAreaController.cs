using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayAreaController : MonoBehaviour, IDropHandler
{
    public static PlayAreaController instance;

    private void Awake()
    {
        instance = this;
        _cards = new List<Card>();
    }

    public OutputController Trade;
    private int _trade;
    public OutputController Combat;
    private int _combat;
    public OutputController Authority;
    private int _authority;

    public Transform CardLayout;

    private List<Card> _cards;
    private Dictionary<Faction, int> _factions;

    private void Start()
    {
        GameManager.instance.OnGameStart += GameStart;
        _factions = new Dictionary<Faction, int>();
        _factions.Add(Faction.Slugs, 0);
        _factions.Add(Faction.Technocult, 0);
        _factions.Add(Faction.StarEmpire, 0);
        _factions.Add(Faction.TradeFederation, 0);
    }

    public void GameStart()
    {
        _cards.Clear();
        _trade = _combat = _authority = 0;
        Trade.UpdateValue(_trade);
        Combat.UpdateValue(_combat);
        Authority.UpdateValue(_authority);
    }

    public void EndTurn()
    {
        _trade = _combat = _authority = 0;
        Trade.UpdateValue(_trade);
        Combat.UpdateValue(_combat);
        Authority.UpdateValue(_authority);
        _cards.Clear();

        switch (TurnManager.instance.Turn)
        {
            case Turn.PlayerTurn:
                _cards.AddRange(PlayerController.instance.Bases.ConvertAll(p => p.Card));
                break;
            case Turn.EnemyTurn:
                break;
            default:
                break;
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
                switch (TurnManager.instance.Turn)
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
            case EffectType.DestroyBase:
                // enemy does not have card with this type of effect
                // and doesnt process here
                break;
            case EffectType.ForceToDiscard:
                // only enemy have this type of effect
                PlayerController.instance.DiscardCard();
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

         _trade-=card.Cost;
        Trade.UpdateValue(_trade);
        return true;
    }

    public void OnDrop(PointerEventData eventData)
    {
        CardController card = eventData.pointerDrag.GetComponent<CardController>();

        if (!card) return;

        if (card.IsBase)
            PlayerController.instance.PlaceBase(card);
        else
            card.Parent = CardLayout;

        if (card.Card.IsChoiceRequired)
        {
            // show options to choose
            // TODO
        }
        else if (card.Card.Effects.FindAll(p => p.Type == EffectType.DestroyBase).Count > 0)
        {
            card.SetActive(true);
            PlayCard(card.Card);
        }
        else
        {
            PlayCard(card.Card);
        }
    }
}