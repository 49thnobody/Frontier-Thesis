using UnityEngine;
using UnityEngine.EventSystems;

public class DropController : MonoBehaviour, IDropHandler
{
    public DropPlaceType Type;
    public void OnDrop(PointerEventData eventData)
    {
        CardController card = GetComponent<CardController>();
        ResourceController resource = GetComponent<ResourceController>();

        if (card)
        {
            if (Type == DropPlaceType.PlayArea)
                card.Parent = transform;

            if (Type == DropPlaceType.ScrapHeap) ;//TODO - scrap a card
        }

        if (resource)
        {
            switch (resource.Type)
            {
                case ResourceType.Combat:
                    if (Type == DropPlaceType.Card) ;//TODO - attack base
                    if (Type == DropPlaceType.EnemyAuthority) ; //TODO - attack enemy
                    break;
                case ResourceType.Trade:
                    if (Type == DropPlaceType.Card) ;// TODO - buy card
                    break;
                default:
                    break;
            }
        }
        //DragController drag = eventData.pointerDrag.GetComponent<DragController>();

            //if (!drag) return;

            //drag.Parent = transform;
    }
}
