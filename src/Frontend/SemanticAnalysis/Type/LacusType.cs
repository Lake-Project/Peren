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
    public abstract bool CanAccept(LacusType type);
}
