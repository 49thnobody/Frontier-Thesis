using UnityEngine;
using UnityEngine.EventSystems;

public class DropController : MonoBehaviour, IDropHandler
{
    // события
    // эта часть мне не очень нравится, но вся проблема в том, 
    // что скрипт, реализующий IDropHandler должен назодится на том объекте, на который будут что-то ронять,
    // а скрипт ScrapController висит на целом канвасе, а мне нужно ронять толкьо на маленькую панельку
    public delegate void ScrapPanelDrop(CardController card);
    public event ScrapPanelDrop OnScrapDrop;

    // тоже самое с  DiscardController
    public delegate void DiscardPanelDrop(CardController card);
    public event DiscardPanelDrop OnDiscardDrop;

    // тип места, на которое могут уронить карту
    public DropPlaceType Type;
    public void OnDrop(PointerEventData eventData)
    {
        CardController card = eventData.pointerDrag.GetComponent<CardController>();
        
        // проверка на null
        if (card)
        {
            // карту уронили на "мусорку" и у этой карты есть утилизационный эффект
            // карту нужно утилизировать
            if (Type == DropPlaceType.ScrapHeap && card.HaveScrapEffect)
            {
                // разыгрываются утилизационные эффекты карты
                foreach (var effect in card.Card.ScrapEffects)
                {
                    PlayAreaController.instance.PlayEffect(effect);
                }

                // утилизация
                card.Scrap();
            }

            // вызов событий
            if (Type == DropPlaceType.ScrapPanel) OnScrapDrop?.Invoke(card);
            if (Type == DropPlaceType.DiscardPanel) OnDiscardDrop?.Invoke(card);
        }
    }
}
