using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    #region Card Info
    public Card Card;
    public CardState State { get; private set; }

    // ���������� � ���, ����� ������� ���� � ���� ����� - ��� ��������
    public bool IsBase => Card == null ? false : Card.Shield != null;
    public bool HaveAlly1Effect => Card.Effects.FindAll(p => p.Group == EffectGroup.Ally1).Count > 0;
    public bool HaveAlly2Effect => Card.Effects.FindAll(p => p.Group == EffectGroup.Ally2).Count > 0;
    public bool HaveScrapEffect => Card.Effects.FindAll(p => p.Group == EffectGroup.Scrap).Count > 0;
    #endregion

    [SerializeField] private CardUIController _shipUI;
    [SerializeField] private CardUIController _baseUI;

    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private Image _image;
    [SerializeField] private Image _faction;
    [SerializeField] private Image _costImage;
    [SerializeField] private TextMeshProUGUI _costText;

    [SerializeField] private Transform _effectsLayout;
    [SerializeField] private EffectGroupController _effectGroupPrefab;

    // ������������ �����
    public void Set(Card card)
    {
        Card = card;

        if (IsBase)
            _baseUI.ShowUI(Card, true);
        else
            _shipUI.ShowUI(Card, false);
    }

    // ��� ����������� ��������� ����� - � ����� ������ ��� ����� ���������� �����
    private CardState _previosSate;
    // ������������� ��������� �����
    public void SetState(CardState state)
    {
        // ���������� ����������� ���������
        if (state == CardState.Panel) _previosSate = State;
        // ����������� ����������� ��������� ���� �������� ��� ������ ��������
        if (state == CardState.Cancel) State = _previosSate;
        State = state;
        // ����� ���������� ������� � ��������� ����� � ����������� �� ����,
        // ��� ��� ���������
        switch (State)
        {
            case CardState.EnemyBuy:
                gameObject.GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f, 1f);
                if (IsBase)
                    transform.eulerAngles = new Vector3(0, 0, 0);
                else
                    transform.eulerAngles = new Vector3(0, 0, 0);
                break;
            case CardState.DiscardPile:
                gameObject.GetComponent<RectTransform>().localScale = new Vector3(0.6f, 0.6f, 1f);
                if (IsBase)
                    transform.eulerAngles = new Vector3(0, 0, 0);
                else
                    transform.eulerAngles = new Vector3(0, 0, 0);
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
            case CardState.Panel:
            case CardState.Selected:
                gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
                transform.Rotate(new Vector3(0, 0, 0));
                break;
            default:
                break;
        }
    }

    // ���������� �����
    public void Scrap()
    {
        // �������� ����� � CardSystem - ���� ������ ��������
        // �� �������� ���� ������ �� ������������� �����
        CardSystem.instance.ToScrap(Card);
        Destroy(gameObject);
    }

    // �������������� ���������� ����� (��������� ��� ������������ ����� ��� ������� Drag&Drop
    // �� ���� ������������ ���� ���� � ���� � ������������ ���� ����������
    public void Place(Transform parent)
    {
        Parent = parent;
        transform.SetParent(parent);
    }

    // ���� ��� � ������ ������� �������� �� �������������� �����
    #region Dragging
    public Transform Parent;
    private Camera _mainCamera;
    private Vector3 _offset;
    // ��� ��� ��������� - ������ ��������� ����� ��-�� ����,
    // ��� � ���������� ���������� Drag&Drop �������� �� ���������
    // ��� ���������� �������� � ����� ���������� �� ���������� ������� OnDrop
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    // ������ �������������� 
    public void OnBeginDrag(PointerEventData eventData)
    {
        // ���� �� ��� ������ - ����� ������������� ������
        if (PlayAreaController.instance.Turn != Turn.PlayerTurn)
        {
            eventData.pointerDrag = null;
            return;
        }

        // ��� �� ���� ����� �� ��������� � ����, ������ ������ ��� � ������� ����, � ��� �� ������ �������������
        if (State != CardState.Hand && State != CardState.Panel && State != CardState.PlayArea && State != CardState.Basement)
        {
            eventData.pointerDrag = null;
            return;
        }

        // ������������� ������� - �������, ����� ����� �� ������������� ������� � �������
        _offset = transform.position - _mainCamera.ScreenToWorldPoint(eventData.position);
        // ����������� ��������
        Parent = transform.parent;
        // ���������� �������� - ������ "��������" � ��������
        transform.SetParent(GetComponentInParent<Canvas>().transform);
        // �� � ���� ��������
        _canvasGroup.blocksRaycasts = false;
    }

    // ��������������
    public void OnDrag(PointerEventData eventData)
    {
        // ����������� ������� �������
        Vector3 pointerPos = _mainCamera.ScreenToWorldPoint(eventData.position);
        // ����������� �����
        transform.position = pointerPos + _offset;
    }

    // ����� ��������������
    // ����� ���� �������� ����������� OnDrop
    public void OnEndDrag(PointerEventData eventData)
    {
        // ���������� �������� ����� (����������� ���� ������, �� �������� � OnDrop)
        transform.SetParent(Parent);
        // ����� ��������� ������ �� ����� (�� ��������� ��������)
        _canvasGroup.blocksRaycasts = true;
    }
    #endregion

    // ���� ������ �������� �� ��������, ����� ���-�� "�������" �� �����
    #region Dropping
    public void OnDrop(PointerEventData eventData)
    {
        // �� ����� ����� "�������" ������ ������
        ResourceController resource = eventData.pointerDrag.GetComponent<ResourceController>();

        if (resource)
        {
            // ���� �� ����� "�������" ���� �������� � ����� ��������� � �������� ����, 
            // � ����� ������
            if (resource.Type == ResourceType.Trade && State == CardState.TradeRow)
            {
                // ��������, ������� �� ����� �������� � ����� ������ ��, ���� �������
                if (PlayAreaController.instance.BuyCard(Card))
                {
                    // ������� ����� ����� �������, ����� Drag&Drop, ������������� ������ �������
                    // �������� ������� ������� �����, ����� �������� ����� � ����� ������
                    PlayerController.instance.OnBuy(Card);
                    // � �������� ����� � �������� ����
                    CardSystem.instance.OnPlayerBuy(this);
                }
            }
            // ���� �� ����� "�������" ���� ��������, ����� �������� ����� � ��� ��������� �� � ����, 
            // � ����� �������
            if (resource.Type == ResourceType.Combat && IsBase && State == CardState.Basement)
            {
                // �������� ������� �� �����
                if (resource.Value < Card.Shield.HP) return;

                // �������� ����� � ������ ������, ��� ��� �����������
                PlayAreaController.instance.DestroyBase(this);
            }
        }
    }
    #endregion
}
