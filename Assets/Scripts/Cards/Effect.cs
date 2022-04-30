public struct Effect
{
    public Target Target;
    public EffectType Type;
    public int Value;
    public bool IsApplied;

    public Effect(Target target,
                  EffectType type,
                  int value)
    {
        Target = target;
        Value = value;
        Type = type;
        IsApplied = false;
    }
}
