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

    private void Start()
    {
        GameManager.instance.OnGameStart += GameStart;
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
                _cards.AddRange(PlayerController.instance.Bases.ConvertAll(p=>p.Card));
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
            switch (effect.Type)
            {
                case EffectType.Trade:
                    _trade += effect.Value;
                    break;
                case EffectType.Combat:
                    _combat += effect.Value;
                    break;
                case EffectType.Authority:
                    _authority += effect.Value;
                    break;
                case EffectType.Draw:
                    switch (TurnManager.instance.Turn)
                    {
                        case Turn.PlayerTurn:
                            PlayerController.instance.DrawCard();
                            break;
                        case Turn.EnemyTurn:
                            EnemyController.instance.DrawCard();
                            break;
                        default:
                            break;
                    }
                    break;
                case EffectType.DestroyBase:
                    // enemy does not have card with this type of effect

                    break;
                case EffectType.ForceToDiscard:
                    break;
                case EffectType.ScrapFromHand:
                    break;
                case EffectType.ScrapFromDiscardPile:
                    break;
                case EffectType.ScrapFromHandOrDiscardPile:
                    break;
                case EffectType.ScrapFromTradeRow:
                    break;
                default:
                    break;
            }
        }

        Trade.UpdateValue(_trade);
        Combat.UpdateValue(_combat);
        Authority.UpdateValue(_authority);
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
        else
        {
            PlayCard(card.Card);
        }
    }
}