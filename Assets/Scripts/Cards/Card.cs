using System.Collections.Generic;

public struct Card
{
    public int Cost;
    public Faction Fraction;
    public List<Effect> Effects;
    public Shield Shield;

    public Card(int cost,
                Faction fraction,
                List<Effect> effects,
                Shield shield = null)
    {
        Cost = cost;
        Fraction = fraction;
        Effects = effects;
        Shield = shield;
        Effects = effects;
    }
}