using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // синглтон
    public static PlayerController instance;

    // авторитет
    [SerializeField] private OutputController _authority;
    // родитель для карт в руке
    [SerializeField] private Transform _handLayout;
    // префаб карты
    [SerializeField] private CardController _cardPrefab;

    // рука (карты и геймобджектс
    #region Hand
    private List<CardController> _hand;
    private List<Card> _handCards;
    public List<Card> HandCards => _handCards;
    public List<CardController> Hand => _hand;
    #endregion

    // родитель для баз
    [SerializeField] private Transform _baseLayout;
    // базы
    public List<CardController> Bases;

    // количество карт в личной колоде
    [SerializeField] private TextMeshProUGUI _deckCardCount;
    // личная колода
    private List<Card> _deck;
    // стопка сброса
    private List<Card> _discardPile;
    public List<Card> DiscardPile => _discardPile;

    // картинка последней карты в стопке сброса
    [SerializeField] private Transform _discardPileTransform;
    private CardController _discardLastCard;

    // кнопка - сыграть всё
    [SerializeField] private Button _playAllCards;

    private void Awake()
    {
        instance = this;
        _discardPile = new List<Card>();
        _hand = new List<CardController>();
        _handCards = new List<Card>();
        _deck = new List<Card>();
        Bases = new List<CardController>();

        _playAllCards.onClick.AddListener(PlayAllCards);
    }

    private void Start()
    {
        GameManager.instance.OnGameStart += GameStart;

        CardSystem.instance.OnScrap += OnScrap;
    }

    // сыграть всё
    private void PlayAllCards()
    {
        for (int i = 0; i < _hand.Count; i++)
        {
            // я решила, что играть на автомате карты, которые открывают панельку выбора, будет не очень
            if (_hand[i].Card.Effects.FindAll(p => p.Type.ToString().Contains("Scrap")).Count > 0)
                continue;
            PlayAreaController.instance.PlayCard(_hand[i]);
        }
    }

    // получение урона
    public void TakeDamage(int damage)
    {
        // невалидное значение
        if (damage < 0) return;
        // получаем урон
        _authority.UpdateValue(_authority.Value - damage);
        // авторитет упал до нуля иди ниже - проиграли
        if (_authority.Value <= 0)
            EndGameController.instance.Show(false);
    }

    // получение авторитета
    public void AddAuthority(int authority)
    {
        // невалидное значение
        if (authority < 0) return;
        // получаем авторитет
        _authority.UpdateValue(_authority.Value + authority);
        // авторитет упал до нуля иди ниже - проиграли
        if (_authority.Value >= 100)
            EndGameController.instance.Show(true);
    }

    // утилизация
    private void OnScrap(Card card)
    {
        // ищем карту везде и удаляем
        if (_discardPile.Contains(card))
            _discardPile.Remove(card);
        if (_hand.ConvertAll(p => p.Card).Contains(card))
        {
            var cardC = _hand.Find(p => p.Card == card);
            _hand.Remove(cardC);
            Destroy(cardC.gameObject);
        }
        if (Bases.ConvertAll(p => p.Card).Contains(card))
        {
            var cardC = Bases.Find(p => p.Card == card);
            Bases.Remove(cardC);
            Destroy(cardC.gameObject);
        }
    }

    // начало игры
    public void GameStart()
    {
        // спавним карту в стопке сброса
        _discardLastCard = Instantiate(_cardPrefab, _discardPileTransform);
        UpdateDiscardImage();
        Reset();
        // копируем себе колоду
        foreach (var card in CardSystem.instance.StartingDeck)
        {
            _deck.Add(card.Clone() as Card);
        }
        // перемешиваем
        Shuffle();

        // берем 5 карт
        for (int i = 0; i < 5; i++)
        {
            DrawCard();
        }
    }
    
    // конец хода
    public void EndTurn()
    {
        // сбрасываем количество сбрасываемых в начале хода карт
        _discardCount = 0;
        // карты с руки в сброс
        _discardPile.AddRange(_handCards.FindAll(p => p.Shield == null));
        // руку очищаем
        _handCards.Clear();

        // удаляем объекты из руки (только не базы) (так уж получилось, что технически они остаются в руке, хоть и отображаются на поле)
        for (int i = _hand.Count - 1; i >= 0; i--)
        {
            if (!_hand[i].IsBase)
                Destroy(_hand[i].gameObject);
            else if (_hand[i].State != CardState.Basement)
                Destroy(_hand[i].gameObject);

            _hand.RemoveAt(i);
        }
        UpdateDiscardImage();

        for (int i = 0; i < 5; i++)
        {
            DrawCard();
        }

        // делаем кнопку сыграть все активной
        _playAllCards.enabled = false;
    }

    // сброс
    private void Reset()
    {
        // сброс авторитета
        _authority.UpdateValue(50);

        // уничтожение объектов в руке
        for (int i = _hand.Count - 1; i >= 0; i--)
        {
            Destroy(_hand[i].gameObject);
            _hand.RemoveAt(i);
        }
        // очищаем руку
        _handCards.Clear();

        // уничтожаем базы
        for (int i = Bases.Count - 1; i >= 0; i--)
        {
            Destroy(Bases[i]);
            Bases.RemoveAt(i);
        }

        // очищаем всё остальное
        _deck.Clear();
        _discardPile.Clear();
        UpdateDiscardImage();
    }

    // взять карту
    public void DrawCard()
    {
        // колода пуста - перемешиваем
        if (_deck.Count == 0)
            Shuffle();

        // берем последнюю карту
        Card card = _deck[_deck.Count - 1];
        // сбрасываем флаг применения эффектов карты
        foreach (var effect in card.Effects)
        {
            effect.IsApplied = false;
        }
        // спавним карту в руке
        CardController cardC = Instantiate(_cardPrefab, _handLayout);
        // инициализируем
        cardC.Set(card);
        // инициализируем состояние
        cardC.SetState(CardState.Hand);
        // добавление карт в списки
        _handCards.Add(card);
        _hand.Add(cardC);
        // убираем карту из колоды
        _deck.RemoveAt(_deck.Count - 1);
        // меняем количество оставшихся карт
        _deckCardCount.text = _deck.Count.ToString();
    }

    // очень интересная часть - противник может заставить игрока сброить карту в начале его хода
    // поэтому тут я просто считаю, сколько карт скинуть
    private int _discardCount;
    public void DiscardOnStart()
    {
        _discardCount++;
    }

    // начало хода
    public void StartTurn()
    {
        // делаем кнопку снова доступной
        _playAllCards.enabled = true;
        // если нам ничег оне нужно скидывать, уходим
        if (_discardCount == 0) return;
        // если кто-то уже выиграл, тоже
        if (EnemyController.instance.Authority.Value <= 0) return;
        if (_authority.Value <= 0) return;
        // вызываем окно сброса карт
        DiscardController.instance.Show(_handCards, _discardCount);
    }

    // сброс карт
    public void Discard(List<Card> cards)
    {
        // каждую карту
        foreach (var card in cards)
        {
            // ищем в руке и сбрасываем
            var cardC = _hand.Find(p => p.Card == card);
            if (cardC != null)
            {
                _hand.Remove(cardC);
                Destroy(cardC.gameObject);
            }
            _handCards.Remove(card);
            _discardPile.Add(card);
        }
    }

    // сброс базы
    public void DiscardBase(CardController @base)
    {
        // ничего нового - кидаем её в сброс и убираем отовсюду
        _discardPile.Add(@base.Card);
        UpdateDiscardImage();
        Bases.Remove(@base);
        Destroy(@base.gameObject);
    }

    // покупка
    public void OnBuy(Card card)
    {
        // добавляем карту в сброс
        _discardPile.Add(card);
        UpdateDiscardImage();
    }

    // обновляем картинку сброса на последнюю карту в стопке
    private void UpdateDiscardImage()
    {
        // если сброс пустой скрываем объект
        if (_discardPile.Count == 0)
        {
            _discardPileTransform.gameObject.SetActive(false);
            return;
        }
        _discardPileTransform.gameObject.SetActive(true);
        _discardLastCard.Set(_discardPile[_discardPile.Count - 1]);
        _discardLastCard.SetState(CardState.DiscardPile);
    }

    // перемешивание
    private void Shuffle()
    {
        // формируем новую колоду
        var allCards = new List<Card>();
        allCards.AddRange(_deck);
        allCards.AddRange(_discardPile);
        // очищаем всё
        _discardPile.Clear();
        UpdateDiscardImage();
        _deck.Clear();
        _deck.AddRange(allCards);
        // перемешивание
        CardSystem.instance.Shuffle(ref _deck);
    }

    // размешение базы
    public void PlaceBase(CardController card)
    {
        Bases.Add(card);
        card.Place(_baseLayout);
        card.SetState(CardState.Basement);
    }
}
