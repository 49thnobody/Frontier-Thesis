using System.Collections.Generic;
using UnityEngine;

public struct Card
{
    public string Name;
    public int Cost;
    public Faction Fraction;
    public List<Effect> Effects;
    public Shield Shield;
    public Sprite Sprite;

    public Card(string name, int cost,
                Faction fraction,
                List<Effect> effects,
                Shield shield = null)
    {
        Name = name;
        Cost = cost;
        Fraction = fraction;
        Effects = effects;
        Shield = shield;
        Effects = effects;
        Sprite = Resources.Load<Sprite>($"Sprites/Cards/{Name}");
    }
}