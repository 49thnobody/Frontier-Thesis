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
        CardController card = GetComponent<CardController>();
        ResourceController resource = GetComponent<ResourceController>();

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

            // hate this part
            if (Type == DropPlaceType.ScrapPanel) OnScrapDrop?.Invoke(card);
            if (Type == DropPlaceType.DiscardPanel) OnDiscardDrop?.Invoke(card);
        }

        if (resource)
        {
            switch (resource.Type)
            {
                case ResourceType.Combat:
                    if (Type == DropPlaceType.EnemyAuthority) ; //TODO - attack enemy
                    break;
                default:
                    break;
            }
        }
    }
}
