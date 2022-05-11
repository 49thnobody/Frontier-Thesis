using UnityEngine;
using UnityEngine.EventSystems;

public class DropController : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        DragController drag = eventData.pointerDrag.GetComponent<DragController>();

        if (!drag) return;

        drag.Parent = transform;
    }
}
