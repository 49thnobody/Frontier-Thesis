using System;

public class Shield : ICloneable
{
    public ShieldType Type;
    public int HP;
    public bool IsPlaced;

    public Shield(ShieldType type,
                  int hP)
    {
        Type = type;
        HP = hP;
        IsPlaced = false;
    }

    public object Clone()
    {
        return new Shield(Type, HP);
    }
}