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
        _dropController = GetComponentInChildren<DropController>();
        OkButton.onClick.AddListener(OkOnClick);
        Canceuttonl.onClick.AddListener(CancelOnClick);
        _dropController.OnScrapDrop += OnDrop;
    }

    private void OnDrop(CardController card)
    {
        card.Parent = SelectedCardLayout;
    }

    private DropController _dropController;

    public GameObject ScrapPanel;
    public TextMeshProUGUI AmountOfCards;
    public Transform AllCardLayout;
    public Transform SelectedCardLayout;
    public CardController CardPrefab;
    public Button OkButton;
    public Button Canceuttonl;

    private Effect _effectReference;
    public void Show(List<Card> cardsToChoose, int amount, Effect effectRef)
    {
        ScrapPanel.SetActive(true);
        _effectReference = effectRef;
        AmountOfCards.text = amount.ToString();
        foreach (var card in cardsToChoose)
        {
            var newCard = Instantiate(CardPrefab, AllCardLayout);
            newCard.Set(card);
            newCard.SetState(CardState.ScrapPanel);
        }
    }

    public void OkOnClick()
    {
        var cardsToScrap = SelectedCardLayout.GetComponentsInChildren<CardController>();

        foreach (var card in cardsToScrap)
        {
            card.Scrap();
        }

        _effectReference.IsApplied = true;
        CancelOnClick();
    }

    private void Reset()
    {
        var selected = SelectedCardLayout.GetComponentsInChildren<CardController>();
        for (int i = selected.Length - 1; i >= 0; i--)
        {
            Destroy(selected[i].gameObject);
        }

        var all = AllCardLayout.GetComponentsInChildren<CardController>();
        for (int i = all.Length - 1; i >= 0; i--)
        {
            Destroy(all[i].gameObject);
        }
    }

    public void CancelOnClick()
    {
        Reset();
        ScrapPanel.SetActive(false);
    }
}
