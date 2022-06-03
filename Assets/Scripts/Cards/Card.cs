using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Card
{
    public string Name;
    public int Cost;
    public Faction Faction;
    public List<Effect> Effects;
    public Shield Shield;
    public Sprite Sprite;

    public List<Effect> PrimaryEffects => Effects.FindAll(p => p.Group == EffectGroup.Priamry);

    public List<Effect> Ally1Effects => Effects.FindAll(p => p.Group == EffectGroup.Ally1);
    public List<Effect> Allly2Effects => Effects.FindAll(p => p.Group == EffectGroup.Ally2);
    public List<Effect> ScrapEffects => Effects.FindAll(p => p.Group == EffectGroup.Scrap);


    public Card(string name, int cost,
                Faction fraction,
                List<Effect> effects,
                Shield shield = null)
    {
        Name = name;
        Cost = cost;
        Faction = fraction;
        Effects = effects;
        Shield = shield;
        Effects = effects;
        Sprite = Resources.Load<Sprite>($"Cards/{Name}");
    }
}