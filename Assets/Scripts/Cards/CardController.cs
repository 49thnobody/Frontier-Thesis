using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    #region Card Info
    public Card Card;
    public CardState State { get; private set; }

    // информация о том, какие эффекты есть у этой карты - для удобства
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

    // иницализация карты
    public void Set(Card card)
    {
        Card = card;

        if (IsBase)
            _baseUI.ShowUI(Card, true);
        else
            _shipUI.ShowUI(Card, false);
    }

    // кэш предыдущего состояния карты - в целом только для отмны утилизации карты
    private CardState _previosSate;
    // инициализация состояния карты
    public void SetState(CardState state)
    {
        // сохранение предыдущего состояния
        if (state == CardState.Panel) _previosSate = State;
        // возвращение предыдущего состояния если действие над картой отменено
        if (state == CardState.Cancel) State = _previosSate;
        State = state;
        // здесь происходит поворот и изменение карты в зависимости от того,
        // где она размещена
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

    // утилизация карты
    public void Scrap()
    {
        // передаем карту в CardSystem - этот скрипт отвечает
        // за удаление всех ссылок на утилизируемую карту
        CardSystem.instance.ToScrap(Card);
        Destroy(gameObject);
    }

    // принудительное размещение карты (требуется для разыгрывания карты без участия Drag&Drop
    // то есть разыгрывание всех карт с руки и разыгрывание карт противника
    public void Place(Transform parent)
    {
        Parent = parent;
        transform.SetParent(parent);
    }

    // весь код в данном регионе отвечает за перетаскивание карты
    #region Dragging
    public Transform Parent;
    private Camera _mainCamera;
    private Vector3 _offset;
    // вот это интересно - данный компонент нужен из-за того,
    // что с элементами интерфейса Drag&Drop работает не корректно
    // без отключения рейкаста у этого компонента не вызовается событие OnDrop
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    // начало перетаскивания 
    public void OnBeginDrag(PointerEventData eventData)
    {
        // если не ход игрока - карты перетаскивать нельзя
        if (PlayAreaController.instance.Turn != Turn.PlayerTurn)
        {
            eventData.pointerDrag = null;
            return;
        }

        // так же если карта не находится в руке, панели выбора или в игровой зоне, её так же нельзя перетаскивать
        if (State != CardState.Hand && State != CardState.Panel && State != CardState.PlayArea && State != CardState.Basement)
        {
            eventData.pointerDrag = null;
            return;
        }

        // инициализация оффсета - отступа, чтобы карта не перескакивала центром к курсору
        _offset = transform.position - _mainCamera.ScreenToWorldPoint(eventData.position);
        // кжширование родителя
        Parent = transform.parent;
        // назначение родителя - самого "верхнего" в иерархии
        transform.SetParent(GetComponentInParent<Canvas>().transform);
        // ну и блок рейкаста
        _canvasGroup.blocksRaycasts = false;
    }

    // перетаскивание
    public void OnDrag(PointerEventData eventData)
    {
        // определение позиции курсора
        Vector3 pointerPos = _mainCamera.ScreenToWorldPoint(eventData.position);
        // перемещение карты
        transform.position = pointerPos + _offset;
    }

    // конец перетаскивания
    // перед этим событием срабатывает OnDrop
    public void OnEndDrag(PointerEventData eventData)
    {
        // назначение родителя назад (предыдущего либо нового, он задается в OnDrop)
        transform.SetParent(Parent);
        // снова разрешаем тыкать на карту (не блокируем рейкасты)
        _canvasGroup.blocksRaycasts = true;
    }
    #endregion

    // этот регион отвечает за ситуации, когда что-то "уронили" на карту
    #region Dropping
    public void OnDrop(PointerEventData eventData)
    {
        // на карту можно "уронить" толькр ресурс
        ResourceController resource = eventData.pointerDrag.GetComponent<ResourceController>();

        if (resource)
        {
            // если на карту "уронили" очки торговли и карта находится в торговом ряду, 
            // её нужно купить
            if (resource.Type == ResourceType.Trade && State == CardState.TradeRow)
            {
                // проверка, хватает ли очков торговли и сразу тратим их, если хватает
                if (PlayAreaController.instance.BuyCard(Card))
                {
                    // покупка карты таким образом, через Drag&Drop, осуществяется только игроком
                    // вызываем событие Покупка карты, чтобы добавить карту в сброс игрока
                    PlayerController.instance.OnBuy(Card);
                    // и обновить карту в торговом ряду
                    CardSystem.instance.OnPlayerBuy(this);
                }
            }
            // если на карту "уронили" очки сражений, карта является базой и она находится не в руке, 
            // её нужно сломать
            if (resource.Type == ResourceType.Combat && IsBase && State == CardState.Basement)
            {
                // проверка хватает ли урона
                if (resource.Value < Card.Shield.HP) return;

                // передача карты в другой скрипт, где она уничтожится
                PlayAreaController.instance.DestroyBase(this);
            }
        }
    }
    #endregion
}
