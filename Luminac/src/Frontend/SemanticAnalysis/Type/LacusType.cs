using System.Runtime.CompilerServices;
using Lexxer;

public enum Range
{
    one_bit,
    eight_bit,
    sixteen_bit,
    thirty_two_bit,
    sixty_four_bit,
    Float,
    none
}

public abstract class LacusType
{
    public LacusType? simplerType { get; set; }
    public Dictionary<string, LacusType> VarainceOfTypes { get; set; }
    public string? name { get; set; }
    public bool IsConst { get; set; } = false;

    public bool IsUnsigned { get; set; } = false;
    public Range Range { get; set; } 

    // public Tokens Op { get; set; } ;

    public LacusType(bool isConst,Range range, bool isUnsigned = false)
    {
        IsConst = isConst;
        IsUnsigned = isUnsigned;
        Range = range;
    }

    public LacusType(string _name, Dictionary<string, LacusType> varainceOfTypes, bool isConst)
    {
        name = _name;
        VarainceOfTypes = varainceOfTypes;
        IsConst = isConst;
    }

    public LacusType(LacusType simplerType, bool isConst, bool isUnsigned = false)
    {
        this.simplerType = simplerType;
        IsConst = isConst;
        IsUnsigned = isUnsigned;
    }

    public abstract bool CanAccept(LacusType type);

    public abstract int size();

    public abstract bool OpAccept(Tokens op);
}