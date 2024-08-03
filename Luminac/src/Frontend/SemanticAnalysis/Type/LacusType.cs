using System.Runtime.CompilerServices;
using Lexxer;

public enum Range
{
    OneBit  = 1,
    EightBit = 256,
    SixteenBit = 65536,
    ThirtyTwoBit = 65537, 
    SixtyFourBit = 65538,
    Float = 65539,
    None = 0
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