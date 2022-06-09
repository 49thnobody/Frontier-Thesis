using UnityEngine;
using UnityEngine.EventSystems;

public class DropController : MonoBehaviour, IDropHandler
{
    public delegate void ScrapPanelDrop(CardController card);
    public event ScrapPanelDrop OnScrapDrop;

    public delegate void DiscardPanelDrop(CardController card);
    public event DiscardPanelDrop OnDiscardDrop;

    public DropPlaceType Type;
    public void OnDrop(PointerEventData eventData)
    {
        CardController card = eventData.pointerDrag.GetComponent<CardController>();
        ResourceController resource = eventData.pointerDrag.GetComponent<ResourceController>();

        if (card)
        {
            if (Type == DropPlaceType.ScrapHeap && card.HaveScrapEffect)
            {
                foreach (var effect in card.Card.ScrapEffects)
                {
                    PlayAreaController.instance.PlayEffect(effect);
                }

                card.Scrap();
            }

            if (Type == DropPlaceType.ScrapPanel) OnScrapDrop?.Invoke(card);
            if (Type == DropPlaceType.DiscardPanel) OnDiscardDrop?.Invoke(card);
        }
    }
}
