using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EffectGroupController : MonoBehaviour
{
    [SerializeField] private Image _type;

    [SerializeField] private HorizontalLayoutGroup _horizontalEffectsLayout;
    [SerializeField] private VerticalLayoutGroup _vereticalEffectsLayout;

    [SerializeField] private Image _imageEffectPefab;
    [SerializeField] private TextMeshProUGUI _textEffectPefab;

    public void Set(List<Effect> effects)
    {
        if (effects == null || effects.Count == 0) return;
        if (effects[0].Group == EffectGroup.None || effects[0].Group == EffectGroup.Priamry)
            _type.enabled = false;
        else
            _type.sprite = Resources.Load<Sprite>($"{effects[0].Group}");

        SetEffects(effects);
    }

    public void Set(List<Effect> effects, Faction faction)
    {
        string sup = "";
        if (effects[0].Group == EffectGroup.Ally2)
        {
            sup = "double";
        }

        _type.sprite = Resources.Load<Sprite>($"{sup}{faction}");

        SetEffects(effects);
    }

    private void SetEffects(List<Effect> effects)
    {
        var textEffects = effects.FindAll(e => e.IsTextEffect);
        if (textEffects.Count == 0)
        {
            _horizontalEffectsLayout.gameObject.SetActive(true);

            foreach (var effect in effects)
            {
                var effectImage = Instantiate(_imageEffectPefab, _horizontalEffectsLayout.transform);
                effectImage.sprite = Resources.Load<Sprite>($"{effect.Type}");
                effectImage.GetComponentInChildren<TextMeshProUGUI>().text = effect.Value.ToString();
            }
        }
        else
        {
            _vereticalEffectsLayout.gameObject.SetActive(true);
            
            foreach (var effect in effects)
            {
                if (effect.IsTextEffect)
                {
                    var effectText = Instantiate(_textEffectPefab, _vereticalEffectsLayout.transform);
                    string v = effect.Value > 1 ? "ы" : "у";
                    switch (effect.Type)
                    {
                        case EffectType.Draw:
                            effectText.text = $"Возьмите {effect.Value} карт{v}";
                            break;
                        case EffectType.Discard:
                            effectText.text = $"Сбросьте {effect.Value} карт{v}";
                            break;
                        case EffectType.ForceToDiscard:
                            effectText.text = $"Противник сбрасывает {effect.Value} карт{v}";
                            break;
                        case EffectType.ScrapFromHand:
                            effectText.text = $"Утилизируйте {effect.Value} карт{v} из вашей руки";
                            break;
                        case EffectType.ScrapFromDiscardPile:
                            effectText.text = $"Утилизируйте {effect.Value} карт{v} из вашей стопки сброса";
                            break;
                        case EffectType.ScrapFromHandOrDiscardPile:
                            effectText.text = $"Утилизируйте {effect.Value} карт{v} из вашей руки или стопки сброса";
                            break;
                        case EffectType.ScrapFromTradeRow:
                            effectText.text = $"Утилизируйте {effect.Value} карт{v} из торгового ряда";
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    var effectImage = Instantiate(_imageEffectPefab, _vereticalEffectsLayout.transform);
                    effectImage.sprite = Resources.Load<Sprite>($"{effect.Type}");
                    effectImage.GetComponentInChildren<TextMeshProUGUI>().text = effect.Value.ToString();
                }
            }
        }
    }
}

