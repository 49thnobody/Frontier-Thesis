using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    #region Card Info
    // card
    public Card Card;
    public CardState State { get; private set; }

    // cash for quick coding
    public bool IsBase => Card == null ? false : Card.Shield != null;
    public bool HaveAlly1Effect => Card.Effects.FindAll(p => p.Group == EffectGroup.Ally1).Count > 0;
    public bool HaveAlly2Effect => Card.Effects.FindAll(p => p.Group == EffectGroup.Ally2).Count > 0;
    public bool HaveScrapEffect => Card.Effects.FindAll(p => p.Group == EffectGroup.Scrap).Count > 0;
    #endregion

    public Image Image;

    public void Set(Card card)
    {
        Card = card;
        Image.sprite = card.Sprite;
    }

    public void SetState(CardState state)
    {
        State = state;
        switch (State)
        {
            case CardState.DiscardPile:
            case CardState.EnemyBuy:
                gameObject.GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f, 1f);
                if (IsBase)
                    transform.eulerAngles = new Vector3(0, 0, -90);
                else
                    transform.eulerAngles = new Vector3(0, 0, 90);
                break;
            case CardState.Basement:
            case CardState.TradeRow:
                gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
                if (IsBase)
                    transform.eulerAngles = new Vector3(0, 0, -90);
                else
                    transform.eulerAngles = new Vector3(0, 0, 90);
                break;
            case CardState.Hand:
            case CardState.PlayArea:
            case CardState.ScrapPanel:
            case CardState.DiscardPanel:
                gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
                transform.Rotate(new Vector3(0, 0, 0));
                break;
            default:
                break;
        }
    }

    public void Scrap()
    {
        CardSystem.instance.ToScrap(Card);
        Destroy(gameObject);
    }

    public void Place(Transform parent)
    {
        transform.SetParent(parent);
    }

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
        if(State!=CardState.Hand)
        {
            eventData.pointerDrag = null;
            return;
        }
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

    #region Dropping
    public void OnDrop(PointerEventData eventData)
    {
        CardController card = eventData.pointerDrag.GetComponent<CardController>();
        ResourceController resource = eventData.pointerDrag.GetComponent<ResourceController>();

        if (resource)
        {
            if (resource.Type == ResourceType.Trade && State == CardState.TradeRow)
            {
                if (PlayAreaController.instance.BuyCard(Card))
                {
                    PlayerController.instance.OnBuy(Card);
                    CardSystem.instance.OnPlayerBuy(this);
                }
            }
            if (resource.Type == ResourceType.Combat && IsBase)
                ; // TODO - damage check and destroy base
        }
    }
    #endregion
}
