using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public OutputController Authority;

    public Transform Hand;
    public CardController CardPrefab;
    private List<CardController> _hand;

    private List<Card> _deck;
    private List<Card> _discardPile;

    private void Awake()
    {
        _discardPile = new List<Card>();
        _hand = new List<CardController>();
    }

    public void GameStart()
    {
        Authority.UpdateValue(50);
        _deck = CardSystem.instance.StartingDeck;
        Shuffle();
    }

    private void Shuffle()
    {
        var newDeck = new List<Card>();
        newDeck.AddRange(_deck);
        newDeck.AddRange(_discardPile);
        _deck = newDeck;
        _discardPile.Clear();
    }
}
