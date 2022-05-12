using UnityEngine;
using UnityEngine.EventSystems;

public class ResourceController : OutputController, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ResourceType Type;

    #region Dragging
    public Transform Parent;
    private Camera _mainCamera;
    private Vector3 _offset;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _canvasGroup = GetComponent<CanvasGroup>();
        Parent = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _offset = transform.position - _mainCamera.ScreenToWorldPoint(eventData.position);
        _canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 newPos = _mainCamera.ScreenToWorldPoint(eventData.position);
        transform.position = newPos + _offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(Parent);
        _canvasGroup.blocksRaycasts = true;
    }
    #endregion
}
