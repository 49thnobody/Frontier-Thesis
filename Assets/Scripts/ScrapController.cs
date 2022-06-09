using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScrapController : MonoBehaviour
{
    public static ScrapController instance;

    private void Awake()
    {
        instance = this;
        _dropController = GetComponentInChildren<DropController>(true);
        _okButton.onClick.AddListener(OkOnClick);
        _cancelButton.onClick.AddListener(CancelOnClick);
        _dropController.OnScrapDrop += OnDrop;
    }

    int _currentAmount;
    private void OnDrop(CardController card)
    {
        card.Place( _selectedCardLayout);
        card.SetState(CardState.Selected);
        _currentAmount++;

        if (_currentAmount == _neededAmount)
        {
            OkOnClick();
        }
    }

    private DropController _dropController;

    [SerializeField] private GameObject _scrapPanel;
    [SerializeField] private TextMeshProUGUI _amountOfCards;
    private int _neededAmount;
    [SerializeField] private Transform _allCardLayout;
    [SerializeField] private Transform _selectedCardLayout;
    [SerializeField] private CardController _cardPrefab;
    [SerializeField] private Button _okButton;
    [SerializeField] private Button _cancelButton;

    private Effect _effectReference;
    public void Show(List<Card> cardsToChoose, int amount, Effect effectRef)
    {
        _scrapPanel.SetActive(true);
        _effectReference = effectRef;
        _neededAmount = amount;
        _amountOfCards.text = amount.ToString();
        foreach (var card in cardsToChoose)
        {
            var newCard = Instantiate(_cardPrefab, _allCardLayout);
            newCard.Set(card);
            newCard.SetState(CardState.Panel);
        }
    }

    public void OkOnClick()
    {
        var cardsToScrap = _selectedCardLayout.GetComponentsInChildren<CardController>();

        foreach (var card in cardsToScrap)
        {
            card.Scrap();
        }

        _effectReference.IsApplied = true;
        CancelOnClick();
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

    public void CancelOnClick()
    {
        for (int i = 0; i < _selectedCardLayout.childCount; i++)
        {
            _selectedCardLayout.GetChild(i).GetComponent<CardController>().SetState(CardState.Cancel);
        }
        Reset();
        _scrapPanel.SetActive(false);
    }
}
