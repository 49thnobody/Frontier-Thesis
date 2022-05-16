using System.Collections.Generic;
using UnityEngine;

public struct Card
{
    public string Name;
    public int Cost;
    public Faction Fraction;
    public List<Effect> Effects;
    public bool IsChoiceRequired;
    public Shield Shield;
    public Sprite Sprite;

    public List<Effect> PrimaryEffects => Effects.FindAll(p => p.Group == EffectGroup.Priamry);

    public List<Effect> Ally1Effects => Effects.FindAll(p => p.Group == EffectGroup.Ally1);
    public List<Effect> Allly2Effects => Effects.FindAll(p => p.Group == EffectGroup.Ally2);
    public List<Effect> ScrapEffects => Effects.FindAll(p => p.Group == EffectGroup.Scrap);


    public Card(string name, int cost,
                Faction fraction,
                List<Effect> effects, bool isChoiceRequired = false,
                Shield shield = null)
    {
        Name = name;
        Cost = cost;
        Fraction = fraction;
        Effects = effects;
        Shield = shield;
        Effects = effects;
        IsChoiceRequired = isChoiceRequired;
        Sprite = Resources.Load<Sprite>($"Cards/{Name}");
    }
}