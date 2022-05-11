using UnityEngine;
using UnityEngine.EventSystems;

public class CardController : MonoBehaviour
{
    #region Card Info
    // card
    private Card _card;
    private CardState _cardState;

    // cash for quick coding
    public bool IsBase => _card.Shield != null;
    public bool HaveFraction1Effect => _card.Effects.FindAll(p => p.Type == EffectType.Ally1).Count > 0;
    public bool HaveFraction2Effect => _card.Effects.FindAll(p => p.Type == EffectType.Ally2).Count > 0;
    public bool HaveScrapEffect => _card.Effects.FindAll(p => p.Type == EffectType.Scrap).Count > 0;
    #endregion
}
