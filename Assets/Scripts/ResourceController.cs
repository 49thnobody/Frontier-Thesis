using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResourceController : OutputController, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ResourceType Type;

    #region Dragging
    private Vector3 _defaultPosition;
    private Camera _mainCamera;
    private Vector3 _offset;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _text = GetComponentInChildren<TextMeshProUGUI>();
        _mainCamera = Camera.main;
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _defaultPosition = transform.position;
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
        transform.position = _defaultPosition;
        _canvasGroup.blocksRaycasts = true;
    }
    #endregion
}
