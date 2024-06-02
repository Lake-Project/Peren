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

public class LacusType
{
    public Dictionary<TypeEnum, int> TypeOrdance =
        new()
        {
            [TypeEnum.BOOL] = 0,
            [TypeEnum.CHAR] = 1,
            [TypeEnum.INTEGER] = 2,
            [TypeEnum.FLOAT] = 3,
        };

    public string Typename { get; set; }
    public TypeEnum Type { get; set; }

    public bool isNative;

    public LacusType(string _typename, TypeEnum _type)
    {
        this.Typename = _typename;
        this.Type = _type;
        isNative = false;
    }

    public LacusType(TypeEnum _type)
    {
        this.Typename = "";
        this.Type = _type;
        isNative = true;
    }

    public override bool Equals(object? obj)
    {
        LacusType l = (LacusType)obj;
        if (isNative)
            return l.Type == Type;
        return l.Type == Type && Typename == l.Typename;
    }

    public bool greaterThen(LacusType type)
    {
        return type.TypeOrdance[type.Type] > type.TypeOrdance[this.Type];
    }

    public bool LessThen(LacusType type)
    {
        return type.TypeOrdance[type.Type] < type.TypeOrdance[this.Type];
    }
}
