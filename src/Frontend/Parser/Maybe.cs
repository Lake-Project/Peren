namespace Lexxer.Parser;

public class Maybe<T>(Tokens type)
{
    public Tokens current;
    public T Value { get; set; }

    public Maybe<T> RemoveList(TokenType t, List<Tokens> list)
    {
        if (list[0].tokenType == t)
        {
            list.RemoveAt(0);
            return this;
        }

        return this;
    }

    public Maybe<T> Bind(TokenType t, List<Tokens> list, Func<Tokens, Maybe<T>, T> func)
    {
        if (type.tokenType == t)
        {
            Tokens c = list[0];
            list.RemoveAt(0);
            T b = func(c, this);
            if (list.Count != 0)
            {
                var a = new Maybe<T>(list[0]);
                a.Value = b;
                return a;
            }

            Value = b;
        }

        return this;
    }

    public Maybe<T> Bind(
        TokenType t,
        List<Tokens> list,
        Func<Tokens, Maybe<T>, T> func,
        Func<Maybe<T>, T> nothing
    )
    {
        if (type.tokenType == t)
        {
            return Bind(t, list, func);
        }

        if (list.Count != 0)
        {
            Value = nothing(this);
        }

        return this;
    }

    public bool LookAhead(TokenType t, List<Tokens> list)
    {
        return type.tokenType == t;
    }

    // public List<T> Many(TokenType stop, List<Tokens> list, Func<T> func)
    // {
    //     List<T> l = new();
    //     while (list[0].tokenType == stop)
    //     {
    //         l.Add(func());
    //     }
    //
    //     return l;
    // }
    //
    //
}
