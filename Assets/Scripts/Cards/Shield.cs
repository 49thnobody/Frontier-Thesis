using System;

public class Shield : ICloneable
{
    public ShieldType Type;
    public int HP;

    public Shield(ShieldType type,
                  int hP)
    {
        Type = type;
        HP = hP;
    }

    public object Clone()
    {
        return new Shield(Type, HP);
    }
}