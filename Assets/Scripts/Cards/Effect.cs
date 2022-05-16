using System.Collections.Generic;

public struct Effect
{
    public EffectGroup Group;
    public EffectType Type;
    public int Value;
    public bool IsApplied;

    public Effect(EffectGroup group, EffectType type,
                  int value)
    {
        Type = type;
        Value = value;
        Group = group;
        IsApplied = false;
    }
}
