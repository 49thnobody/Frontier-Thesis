using System.Collections.Generic;

public struct Card
{
    public int Cost;
    public Fraction Fraction;
    public List<Effect> Effects;
    public Shield Shield;
    public bool IsBasement => Shield != null;
    public bool HaveFraction1Effect => Effects.FindAll(p => p.Type == EffectType.Fractional1).Count > 0;
    public bool HaveFraction2Effect => Effects.FindAll(p => p.Type == EffectType.Fractional2).Count > 0;
    public bool HaveDisposeEffect => Effects.FindAll(p => p.Type == EffectType.Dispose).Count > 0;

    public Card(int cost,
                Fraction fraction,
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