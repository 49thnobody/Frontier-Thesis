using UnityEngine;
using UnityEngine.EventSystems;

public class CardController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    #region Card Info
    // card
    private Card _card;
    private CardState _cardState;

    // cash for quick coding
    public bool IsBase => _card.Shield != null;
    public bool HaveAlly1Effect => _card.Effects.FindAll(p => p.Type == EffectType.Ally1).Count > 0;
    public bool HaveAlly2Effect => _card.Effects.FindAll(p => p.Type == EffectType.Ally2).Count > 0;
    public bool HaveScrapEffect => _card.Effects.FindAll(p => p.Type == EffectType.Scrap).Count > 0;
    #endregion

    #region Dragging
    public Transform Parent;
    private Camera _mainCamera;
    private Vector3 _offset;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _offset = transform.position - _mainCamera.ScreenToWorldPoint(eventData.position);
        Parent = transform.parent;
        transform.SetParent(Parent.parent);
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
