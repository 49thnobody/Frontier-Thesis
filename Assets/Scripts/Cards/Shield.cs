public class Shield
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
}