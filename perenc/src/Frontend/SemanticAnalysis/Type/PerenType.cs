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

public abstract class PerenType
{
    public PerenType? simplerType { get; set; }
    public Dictionary<string, PerenType> VarainceOfTypes { get; set; }
    public string? name { get; set; }
    public bool IsConst { get; set; } = false;

    public bool IsUnsigned { get; set; } = false;
    public Range Range { get; set; } 

    // public Tokens Op { get; set; } ;

    public PerenType(bool isConst,Range range, bool isUnsigned = false)
    {
        IsConst = isConst;
        IsUnsigned = isUnsigned;
        Range = range;
    }

    public PerenType(string _name, Dictionary<string, PerenType> varainceOfTypes, bool isConst)
    {
        name = _name;
        VarainceOfTypes = varainceOfTypes;
        IsConst = isConst;
    }

    public PerenType(PerenType simplerType, bool isConst, bool isUnsigned = false)
    {
        this.simplerType = simplerType;
        IsConst = isConst;
        IsUnsigned = isUnsigned;
    }

    public abstract bool CanAccept(PerenType type);

    public abstract int size();

    public abstract bool OpAccept(Tokens op);
}