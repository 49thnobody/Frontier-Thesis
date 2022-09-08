using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Card : ICloneable
{
    public string Name;
    public int Cost;
    public Faction Faction;
    public List<Effect> Effects;
    public Shield Shield;

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
    }

    public object Clone()
    {
        var effects = new List<Effect>();
        foreach (Effect effect in Effects)
        {
            effects.Add(effect.Clone() as Effect);
        }
        return new Card(Name, Cost, Faction, effects, Shield == null ? null : Shield.Clone() as Shield);
    }
}