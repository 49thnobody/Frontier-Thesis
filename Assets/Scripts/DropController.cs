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

            if (Type == DropPlaceType.ScrapHeap) ;//TODO - scrap a card
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
