public enum TypeEnum
{
    INTEGER,
    FLOAT,
    CHAR,
    STRUCT,
    POINTER,
    VOID,
    BOOL
}

public abstract class LacusType
{
    public LacusType? simplerType { get; set; }
    public List<LacusType> VarainceOfTypes { get; set; }
    public string? name { get; set; }

    public LacusType() { }

    public LacusType(string _name, List<LacusType> varainceOfTypes)
    {
        name = _name;
        VarainceOfTypes = varainceOfTypes;
    }

    public LacusType(LacusType simplerType)
    {
        this.simplerType = simplerType;
    }

    public abstract bool CanAccept(LacusType type);
}
