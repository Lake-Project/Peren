using System.Runtime.CompilerServices;
using Lexxer;

public enum Range
{
    one_bit  = 1,
    eight_bit = 256,
    sixteen_bit = 65536,
    thirty_two_bit = 65537, 
    sixty_four_bit = 65538,
    Float = 65539,
    none = 0
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