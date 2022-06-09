using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayAreaController : MonoBehaviour, IDropHandler
{
    // синглтон
    public static PlayAreaController instance;

    private void Awake()
    {
        instance = this;
        _cards = new List<Card>();
    }

    // очки торговли
    [SerializeField] private OutputController _trade;
    // очки сражений
    [SerializeField] private OutputController _combat;
    // очки авторитета
    [SerializeField] private OutputController _authority;

    // родетель для разыгранных карт
    [SerializeField] private Transform _cardLayout;
    // префаб карты
    [SerializeField] private CardController CardPrefab;

    // кнопка конец хода
    [SerializeField] private Button ButtonEndTurn;

    // разыгранные карты
    private List<Card> _cards;
    // количество карт фракций
    private Dictionary<Faction, int> _factions;

    // чей ход
    private Turn _turn;
    public Turn Turn => _turn;

    private void Start()
    {
        ButtonEndTurn.onClick.AddListener(OnEndTurn);
        GameManager.instance.OnGameStart += GameStart;
        // данный словарь нужен для учета количества карт фракций
        // а учитывать это нужно для применения фракционных эффектов
        _factions = new Dictionary<Faction, int>();
        _factions.Add(Faction.Slugs, 0);
        _factions.Add(Faction.Technocult, 0);
        _factions.Add(Faction.StarEmpire, 0);
        _factions.Add(Faction.TradeFederation, 0);
    }

    // начало игры
    public void GameStart()
    {
        _turn = Turn.PlayerTurn;
        Reset();
        // игрок ходит первым
        OnStartTurn();
    }

    // начало хода
    private void OnStartTurn()
    {
        // в зависимости от того, чей ход
        switch (_turn)
        {
            case Turn.PlayerTurn:
                // сброс поля
                Reset();
                // берем базы игрока
                var basesP = PlayerController.instance.Bases.ConvertAll(p => p.Card);

                // сбрасываем флаг применения и используем эффекты
                foreach (var @base in basesP)
                {
                    foreach (var effect in @base.Effects)
                    {
                        effect.IsApplied = false;
                    }
                    PlayCard(@base);
                }

                // передаем управление в другой скрипт
                PlayerController.instance.StartTurn();
                break;
            case Turn.EnemyTurn:
                // сброс поля
                Reset();
                // берем базы врага
                var basesE = EnemyController.instance.Bases.ConvertAll(p => p.Card);

                // сбрасываем флаг применения и используем эффекты
                foreach (var @base in basesE)
                {
                    foreach (var effect in @base.Effects)
                    {
                        effect.IsApplied = false;
                    }
                    PlayCard(@base);
                }

                // передаем управление в другой скрипт
                EnemyController.instance.StartTurn();
                break;
            default:
                break;
        }
    }

    // конец хода
    public void OnEndTurn()
    {
        // в зависимости от того, чей ход
        switch (_turn)
        {
            case Turn.PlayerTurn:
                // ищем базы-аванпосты
                var enemyOutposts = EnemyController.instance.Bases.FindAll(p => p.Card.Shield.Type == ShieldType.Outpost);
                // ломаем их
                foreach (var outpost in enemyOutposts)
                {
                    // проверяем, хватает ли урона
                    if (outpost.Card.Shield.HP > _combat.Value)
                    {
                        // не хватает - не пробили защиту, всё
                        _combat.UpdateValue(0);
                        break;
                    }
                    else
                    {
                        // хватает, ломаем базу, уменьшаем очки сражений
                        _combat.UpdateValue(_combat.Value - outpost.Card.Shield.HP);
                        EnemyController.instance.DiscardBase(outpost);
                    }
                }

                // наносим урон врагу на оставшиеся очки сражений
                EnemyController.instance.TakeDamage(_combat.Value);
                // прибавляем себе очки авторитета
                PlayerController.instance.AddAuthority(_authority.Value);
                // вызываем метод Конец хода
                PlayerController.instance.EndTurn();
                // меняем ход
                _turn = Turn.EnemyTurn;
                // и отключаем игроку кнопку конец хода - пусть ждёт, пока босс походит и НИЧЕГО не трогает
                ButtonEndTurn.enabled = false;
                // вызываем начало хода
                OnStartTurn();
                break;
            case Turn.EnemyTurn:
                // ищем базы-аванпосты
                var playerOutposts = PlayerController.instance.Bases.FindAll(p => p.Card.Shield.Type == ShieldType.Outpost);
                // ломаем их
                foreach (var outpost in playerOutposts)
                {
                    // проверяем, хватает ли урона
                    if (outpost.Card.Shield.HP > _combat.Value)
                    {
                        // не хватает - не пробили защиту, всё
                        _combat.UpdateValue(0);
                        break;
                    }
                    else
                    {
                        // хватает, ломаем базу, уменьшаем очки сражений
                        _combat.UpdateValue(_combat.Value - outpost.Card.Shield.HP);
                        PlayerController.instance.DiscardBase(outpost);
                    }
                }

                // наносим урон игроку на оставшиеся очки сражений
                PlayerController.instance.TakeDamage(_combat.Value);
                // меняем ход
                _turn = Turn.PlayerTurn;
                // включаем кнопку
                ButtonEndTurn.enabled = true;
                // вызываем начало хода
                OnStartTurn();
                break;
            default:
                break;
        }
    }

    // уничтожение базы
    public void DestroyBase(CardController @base)
    {
        // хватает ли урона проверяется до вызова этого метода
        // так что просто уменьшаем очки сражений
        _combat.UpdateValue(_combat.Value - @base.Card.Shield.HP);
        // убираем базу из списка разыгранных карт
        _cards.Remove(@base.Card);
        // ломаем базу врага
        // этот метод будет вызываться только игроком с помощью Drag&Drop
        EnemyController.instance.DiscardBase(@base);
    }

    // противник покупает карты
    public void EnemyBuy()
    {
        int trade = _trade.Value;
        // пока есть очки торговли
        do
        {
            // ищем и покупаем самую дороую карту
            CardSystem.instance.BuyMostExpansive(ref trade);
        } while (trade > 0);

        _trade.UpdateValue(trade);
    }

    // сброс
    private void Reset()
    {
        // обнучаем всякие очки
        _trade.UpdateValue(0);
        _combat.UpdateValue(0);
        _authority.UpdateValue(0);
        // очищаем список разыгранных карт
        _cards.Clear();
        // обнуляем карты фракций
        _factions[Faction.Slugs] = 0;
        _factions[Faction.Technocult] = 0;
        _factions[Faction.StarEmpire] = 0;
        _factions[Faction.TradeFederation] = 0;

        // уничтожаем все карты расположенные в игровой зоне
        var cardCs = _cardLayout.GetComponentsInChildren<CardController>();
        for (int i = cardCs.Length - 1; i >= 0; i--)
        {
            Destroy(cardCs[i].gameObject);
        }
    }

    // разыгрывание карты (скорее, размещение)
    public void PlayCard(CardController cardC)
    {
        var card = cardC.Card;

        // если это не база
        if (!cardC.IsBase)
        {
            cardC.Place(_cardLayout);
            cardC.SetState(CardState.PlayArea);
        }
        // если это база, но не размещенная
        else if (cardC.State != CardState.Basement)
        {
            switch (_turn)
            {
                case Turn.PlayerTurn:
                    // размещаем
                    PlayerController.instance.PlaceBase(cardC);
                    break;
                case Turn.EnemyTurn:
                    // размещаем
                    EnemyController.instance.PlaceBase(cardC);
                    break;
                default:
                    break;
            }
        }

        // ещё разыгрывание карты)))
        PlayCard(card);
    }

    // разыгрывание карты
    public void PlayCard(Card card)
    {
        // добавляем к разыгранным картам
        _cards.Add(card);
        // применяем обычный эффекты
        foreach (var effect in card.PrimaryEffects)
        {
            PlayEffect(effect);
        }

        // если карта принадлежит к фракции
        if (card.Faction != Faction.None)
        {
            // отмечаем это
            _factions[card.Faction]++;

            // раызгрываем союзные эффекты
            PlayFactionEffects();
        }
    }

    // применение союзных эффектов
    private void PlayFactionEffects()
    {
        // находим карты с союзным эффектом
        // нет необходимости отдельно искать карты с двойным союзным эффектом
        // потому что если у карты есть двойной эффект, одинарный тоже есть
        var cardsWithFactionEffect = _cards.FindAll(p => p.Ally1Effects.Count != 0);
        // каждую проверяем
        foreach (var card in cardsWithFactionEffect)
        {
            // смотрим есть ли хотя бы 2 карты той же фракции, что и карта
            if (_factions[card.Faction] > 1)
            {
                // применяем союзное свойство
                foreach (var effect in card.Ally1Effects)
                {
                    PlayEffect(effect);
                }
            }

            // смотрим есть ли хотя бы 3 карты той же фракции, что и карта
            if (_factions[card.Faction] > 2)
            {
                // применяем двойное союзное свойство
                foreach (var effect in card.Allly2Effects)
                {
                    PlayEffect(effect);
                }
            }
        }
    }

    // применение эффекта
    public void PlayEffect(Effect effect)
    {
        // если эффект уже применен - до свидания
        if (effect.IsApplied) return;

        // разыгрываем эффект в зависимости от его типа
        switch (effect.Type)
        {
            case EffectType.Trade:
                _trade.UpdateValue(_trade.Value + effect.Value);

                effect.IsApplied = true;
                break;
            case EffectType.Combat:
                _combat.UpdateValue(_combat.Value + effect.Value);

                effect.IsApplied = true;
                break;
            case EffectType.Authority:
                _authority.UpdateValue(_authority.Value + effect.Value);

                effect.IsApplied = true;
                break;
            case EffectType.Draw:
                switch (_turn)
                {
                    case Turn.PlayerTurn:
                        PlayerController.instance.DrawCard();
                        effect.IsApplied = true;
                        break;
                    case Turn.EnemyTurn:
                        EnemyController.instance.DrawCard();
                        effect.IsApplied = true;
                        break;
                    default:
                        break;
                }
                break;
            case EffectType.Discard:
                // у игрока нет карт с таким эффектом, он применяет его на себе (Эффект "Возьмите 1 карту, затем сбросьте 1 карту)Ц
                EnemyController.instance.DiscardCard();
                break;
            case EffectType.ForceToDiscard:
                // у игрока нет карт с таким эффектом, применяем на игрока - заставляем его сбросить карту в начале хода
                PlayerController.instance.DiscardOnStart();
                effect.IsApplied = true;
                break;
                // дальше очень интересная группа эффектов - утилизационная
                // все эти эффекты похожи - сформировать список карт, из которых игрок будет выбирать карты
                // вызов панели утилизации, и всё
                // само применение этого эффекта происходит не здесь, а в ScrapController
            case EffectType.ScrapFromHand:
                List<Card> cardsToSelectHand = new List<Card>();
                cardsToSelectHand.AddRange(PlayerController.instance.Hand.
                    FindAll(p => p.State == CardState.Hand).ConvertAll(p => p.Card));

                ScrapController.instance.Show(cardsToSelectHand, effect.Value, effect);
                break;
            case EffectType.ScrapFromDiscardPile:
                List<Card> cardsToSelectDiscard = new List<Card>();
                cardsToSelectDiscard.AddRange(PlayerController.instance.DiscardPile);

                ScrapController.instance.Show(cardsToSelectDiscard, effect.Value, effect);
                break;
            case EffectType.ScrapFromHandOrDiscardPile:
                List<Card> cardsToSelectAny = new List<Card>();
                cardsToSelectAny.AddRange(PlayerController.instance.DiscardPile);
                cardsToSelectAny.AddRange(PlayerController.instance.Hand.
                    FindAll(p => p.State == CardState.Hand).ConvertAll(p => p.Card));

                ScrapController.instance.Show(cardsToSelectAny, effect.Value, effect);
                break;
            case EffectType.ScrapFromTradeRow:
                List<Card> cardsToSelectTrade = new List<Card>();
                cardsToSelectTrade.AddRange(CardSystem.instance.TradeRow);

                ScrapController.instance.Show(cardsToSelectTrade, effect.Value, effect);
                break;
            default:
                break;
        }
    }

    // покупка карты
    public bool BuyCard(Card card)
    {
        // на самом деле не столько покупка, сколько трата денег - остальное обрабатывается не здесь...
        if (_trade.Value < card.Cost) return false;

        _trade.UpdateValue(_trade.Value - card.Cost);
        return true;
    }

    // разыгрывание карты врага
    public void PlaceEnemyCard(Card card)
    {
        // создаем карту
        CardController newCard = Instantiate(CardPrefab);
        // инициализируем
        newCard.Set(card);
        // если это база - передаем на размещение в скрипт
        if (newCard.IsBase)
            EnemyController.instance.PlaceBase(newCard);
        else
        {
            // не база - размещаем в игровой зоне
            newCard.Place(_cardLayout);
            newCard.SetState(CardState.PlayArea);
        }

        // и играем карту как обычно
        PlayCard(card);
    }

    // IDropHandler
    #region Dropping
    public void OnDrop(PointerEventData eventData)
    {
        CardController card = eventData.pointerDrag.GetComponent<CardController>();

        if (!card) return;

        PlayCard(card);
    }
    #endregion
}