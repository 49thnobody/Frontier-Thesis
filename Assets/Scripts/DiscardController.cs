using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiscardController : MonoBehaviour
{
    public static DiscardController instance;

    private void Awake()
    {
        instance = this;
        _dropController = GetComponentInChildren<DropController>(true);
        _dropController.OnDiscardDrop += OnDrop;
    }

    int _currentAmount;
    private void OnDrop(CardController card)
    {
        card.Place(_selectedCardLayout);
        card.SetState(CardState.Selected);
        _currentAmount++;

        if(_currentAmount == _neededAmount)
        {
            PlayerController.instance.Discard
            (_selectedCardLayout.GetComponentsInChildren<CardController>().ToList().ConvertAll(p => p.Card));
            _discardPanel.SetActive(false);
        }
    }

    private DropController _dropController;

    [SerializeField] private GameObject _discardPanel;
    [SerializeField] private Transform _allCardLayout;
    [SerializeField] private Transform _selectedCardLayout;
    [SerializeField] private CardController _cardPrefab;
    [SerializeField] private TextMeshProUGUI _amountOfCards;
    private int _neededAmount;

    public void Show(List<Card> cardsToChoose, int amount)
    {
        _discardPanel.SetActive(true);
        _neededAmount = amount;
        _currentAmount = 0;
        _amountOfCards.text = _neededAmount.ToString();
        Reset();
        foreach (var card in cardsToChoose)
        {
            var newCard = Instantiate(_cardPrefab, _allCardLayout);
            newCard.Set(card);
            newCard.SetState(CardState.Panel);
        }
    }

    private void Reset()
    {
        for (int i = _selectedCardLayout.childCount - 1; i >= 0; i--)
        {
            Destroy(_selectedCardLayout.GetChild(i).gameObject);
        }
        for (int i = _allCardLayout.childCount - 1; i >= 0; i--)
        {
            Destroy(_allCardLayout.GetChild(i).gameObject);
        }
    }
}
