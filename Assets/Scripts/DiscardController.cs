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
        OkButton.onClick.AddListener(OkOnClick);
        _dropController = GetComponentInChildren<DropController>();
        _dropController.OnDiscardDrop += OnDrop;
    }

    private void OnDrop(CardController card)
    {
        card.Parent = SelectedCardLayout;
    }

    private DropController _dropController;

    public GameObject DiscardPanel;
    public Transform AllCardLayout;
    public Transform SelectedCardLayout;
    public CardController CardPrefab;
    public TextMeshProUGUI AmountOfCards;
    private int _amount;

    public Button OkButton;

    public void Show(List<Card> cardsToChoose, int amount)
    {
        DiscardPanel.SetActive(true);
        OkButton.interactable = false;
        _amount = amount;
        AmountOfCards.text = _amount.ToString();
        foreach (var card in cardsToChoose)
        {
            var newCard = Instantiate(CardPrefab, AllCardLayout);
            newCard.Set(card);
            newCard.SetState(CardState.ScrapPanel);
        }
    }

    private void OkOnClick()
    {
        PlayerController.instance.Discard
            (SelectedCardLayout.GetComponentsInChildren<CardController>().ToList().ConvertAll(p => p.Card));
    }

    private void Update()
    {
        if (SelectedCardLayout.childCount > _amount)
            OkButton.interactable = true;
        else
            OkButton.interactable = false;
    }
}
