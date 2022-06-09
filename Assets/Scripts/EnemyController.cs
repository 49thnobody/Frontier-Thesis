using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // синглтон
    public static EnemyController instance;

    // место, где располагаются базы
    public Transform BaseLayout;
    // список баз
    public List<CardController> Bases;

    // очки авторитета
    public OutputController Authority;

    // префаб карты
    public CardController CardPrefab;
    // карты в руке
    private List<Card> _handCards;

    // личная колода
    private List<Card> _deck;
    // стопка сброса
    private List<Card> _discardPile;

    private void Awake()
    {
        instance = this;
        _discardPile = new List<Card>();
        _handCards = new List<Card>();
        _deck = new List<Card>();
        Bases = new List<CardController>();
    }

    private void Start()
    {
        // подписка на событие - начало игры
        GameManager.instance.OnGameStart += GameStart;
    }

    // конец
    private void EndTurn()
    {
        // сброс карт
        _discardPile.AddRange(_handCards);
        // очистка руки
        _handCards.Clear();

        // набор новых карт 
        for (int i = 0; i < 5; i++)
        {
            DrawCard();
        }

        // вызов Конца хода - для передачи хода игроку
        PlayAreaController.instance.OnEndTurn();
    }

    // взять карту
    public void DrawCard()
    {
        // если в колоде не осталось карт - перемешиваем её
        if (_deck.Count == 0)
            Shuffle();

        // берем карту
        int last = _deck.Count - 1;
        _handCards.Add(_deck[last]);
        // обновляем её эффекты - они в этом ходу ещё точно не применялись
        foreach (var effect in _deck[last].Effects)
        {
            effect.IsApplied = false;
        }
        // убираем карту из колоды
        _deck.RemoveAt(last);
        // сортируем руку - босс будет разыгрывать карты с конца, и разыгрывает он их
        // в порядке убывания стомости
        _handCards.Sort(delegate (Card card1, Card card2)
        {
            return card1.Cost.CompareTo(card2.Cost);
        });
    }

    // сброс карты - слишком легко
    private void Discard(Card card)
    {
        if (_handCards.Contains(card))
        {
            _handCards.Remove(card);
            _discardPile.Add(card);
        }
    }

    // размещение базы
    public void PlaceBase(CardController card)
    {
        Bases.Add(card);
        card.Place(BaseLayout);
        card.SetState(CardState.Basement);
    }

    // сброс базы - её сломали
    public void DiscardBase(CardController basement)
    {
        // добавляем ее в сброс
        _discardPile.Add(basement.Card);
        // убираем из списка
        Bases.Remove(basement);
        // уничтожаем объект
        Destroy(basement.gameObject);
    }

    // получение урона
    public void TakeDamage(int damage)
    {
        // невалидное значение
        if (damage < 0) return;
        // снижаем авторитет
        Authority.UpdateValue(Authority.Value - damage);
        // авторитет упал до нуля или ниже - противник проиграл
        if (Authority.Value <= 0)
            EndGameController.instance.Show(true);
    }

    // перемешивание
    private void Shuffle()
    {
        // собираем новую колоду
        var allCards = new List<Card>();
        allCards.AddRange(_deck);
        allCards.AddRange(_discardPile);
        _discardPile.Clear();
        _deck.AddRange(allCards);
        // отдаем её в метод для перемешивания
        CardSystem.instance.Shuffle(ref _deck);
    }

    // начало хода
    public void StartTurn()
    {
        // просто запуск корутины
        StartCoroutine(OnMyTurn());
    }

    // мой ход
    public IEnumerator OnMyTurn()
    {
        // пока в руке есть карты
        while (_handCards.Count > 0)
        {
            // разыгрываем последнюю карту
            PlayAreaController.instance.PlaceEnemyCard(_handCards[_handCards.Count - 1]);
            Discard(_handCards[_handCards.Count - 1]);
            // чуть чуть ждём
            yield return new WaitForSeconds(1);
        }

        // покупаем карты
        PlayAreaController.instance.EnemyBuy();
        // конец хода
        EndTurn();
    }

    // покупка карты
    public void OnBuy(Card card)
    {
        _discardPile.Add(card);
    }

    // начало игры 
    public void GameStart()
    {
        // сбрасываем всё в ноль 
        Reset();
        // копируем себе стартовую колоду
        foreach (var card in CardSystem.instance.StartingDeck)
        {
            _deck.Add(card.Clone() as Card);
        }

        // перемешиваем
        Shuffle();
        // тянем 5 карт
        for (int i = 0; i < 5; i++)
        {
            DrawCard();
        }
    }

    // сброс до дефолтных значений
    private void Reset()
    {
        // очки авторитета
        Authority.UpdateValue(40);

        // очищаем руку
        _handCards.Clear();
        // стопку сброса
        _discardPile.Clear(); 
        // и колоду
        _deck.Clear();

        // уничтожаем свои базы
        for (int i = Bases.Count - 1; i >= 0; i--)
        {
            Destroy(Bases[i]);
            Bases.RemoveAt(i);
        }
    }

    // сброс карты
    public void DiscardCard()
    {
        if (_handCards.Count == 0) return;

        // ищем самую дешевую карту
        // колода уже отсортирована по вохрастанию
        // поэтому мы просто ищем ВСЕ карты с наименьшей стоимостью
        int i = 0;
        var cardsToDiscard = new List<Card>();
        do
        {
            cardsToDiscard.Add(_handCards[i]);
            i++;
        } while (i < _handCards.Count && _handCards[i].Cost == _handCards[i - 1].Cost);

        // берем случайную карту из числа самых дешевых
        int index = UnityEngine.Random.Range(0, cardsToDiscard.Count);

        // сбрасываем её
        Discard(cardsToDiscard[index]);
    }
}
