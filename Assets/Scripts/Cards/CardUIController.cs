using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private Image _image;
    [SerializeField] private Image _faction;
    [SerializeField] private Image _costImage;
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private Image _shieldImage;
    [SerializeField] private TextMeshProUGUI _shieldText;

    [SerializeField] private Transform _effectsLayout;
    [SerializeField] private EffectGroupController _effectGroupPrefab;

    public void ShowUI(Card card, bool isBase)
    {
        gameObject.SetActive(true);
        _name.text = card.Name;

        if(isBase)
        _image.sprite = Resources.Load<Sprite>($"faceBase{card.Faction}");
        else 
            _image.sprite = Resources.Load<Sprite>($"face{card.Faction}");

        if (card.Faction == Faction.None)
            _faction.enabled = false;
        else
            _faction.sprite = Resources.Load<Sprite>($"{card.Faction}");

        if (card.Cost == 0)
            _costImage.enabled = false;
        else
            _costText.text = card.Cost.ToString();

        var primaries = Instantiate(_effectGroupPrefab, _effectsLayout);
        primaries.Set(card.PrimaryEffects);
        if (card.Ally1Effects.Count > 0)
        {
            var ally1 = Instantiate(_effectGroupPrefab, _effectsLayout);
            ally1.Set(card.Ally1Effects, card.Faction);
        }
        if (card.Allly2Effects.Count > 0)
        {
            var ally2 = Instantiate(_effectGroupPrefab, _effectsLayout);
            ally2.Set(card.Allly2Effects, card.Faction);
        }
        if (card.ScrapEffects.Count > 0)
        {
            var scrapEffects = Instantiate(_effectGroupPrefab, _effectsLayout);
            scrapEffects.Set(card.ScrapEffects);
        }

        if (!isBase) return;
        switch (card.Shield.Type)
        {
            case ShieldType.None:
                _shieldImage.enabled = false;
                break;
            case ShieldType.White:
            case ShieldType.Outpost:
                _shieldImage.enabled = true;
                _shieldImage.sprite = Resources.Load<Sprite>($"{card.Shield.Type}Shield");
                _shieldText.text = card.Shield.HP.ToString();
                _shieldText.color = card.Shield.Type == ShieldType.White ? Color.black : Color.white;
                break;
            default:
                break;
        }
    }
}