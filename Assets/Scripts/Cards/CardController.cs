using UnityEngine;

public class CardController : MonoBehaviour
{
    // card
    private Card _card;
    private CardState _cardState;

    // cash for quick coding
    public bool IsBasement => _card.IsBasement;
    public bool HaveFraction1Effect => _card.HaveFraction1Effect;
    public bool HaveFraction2Effect => _card.HaveFraction2Effect;
    public bool HaveDisposeEffect => _card.HaveDisposeEffect;

    //ref to go
    private GameObject _go;

    private void Awake()
    {
        _go = gameObject;
    }
}
